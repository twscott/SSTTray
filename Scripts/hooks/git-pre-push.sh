#!/bin/sh
# Pre-push hook: scan for hardcoded secrets + check protected file modifications.
# Exit 1 = block push, Exit 0 = allow.

PASS=0
FAIL=1

# ── 1. Collect pushed files ──────────────────────────────────────────
# pre-push receives lines on stdin: <local-ref> <local-sha1> <remote-ref> <remote-sha1>
# We build a list of files unique across all pushed refs.

CHANGED_FILES=""
while read local_ref local_sha remote_ref remote_sha; do
  # Skip deleted refs
  if [ "$local_sha" = "0000000000000000000000000000000000000000" ]; then
    continue
  fi

  # Determine the comparison base
  if [ "$remote_sha" = "0000000000000000000000000000000000000000" ]; then
    # New branch: diff against the common ancestor with the default branch
    BASE=$(git merge-base HEAD origin/HEAD 2>/dev/null || git rev-list --max-parents=0 HEAD | tail -1)
  else
    BASE=$remote_sha
  fi

  FILES=$(git diff --name-only "$BASE".."$local_sha" 2>/dev/null)
  if [ -n "$FILES" ]; then
    CHANGED_FILES=$(printf "%s\n%s" "$CHANGED_FILES" "$FILES")
  fi
done

if [ -z "$CHANGED_FILES" ]; then
  exit $PASS
fi

# Deduplicate
CHANGED_FILES=$(echo "$CHANGED_FILES" | sort -u)

# ── 2. Check for protected file modifications ────────────────────────
HOOKS_DIR=$(dirname "$0")
PROTECTED_FILE="$HOOKS_DIR/protected-files.txt"

if [ -f "$PROTECTED_FILE" ]; then
  while IFS= read -r line; do
    # Skip comments and empty lines
    case "$line" in
      \#*|"") continue ;;
    esac

    # Convert glob pattern to grep pattern: ** -> .*, * -> [^/]*
    PATTERN=$(echo "$line" | sed 's|\\|/|g; s|\*\*|.*|g; s|\*|[^/]*|g')

    PROTECTED=$(echo "$CHANGED_FILES" | grep -E "$PATTERN" || true)
    if [ -n "$PROTECTED" ]; then
      echo "!! PROTECTED: The following files match a protected pattern:"
      echo "$PROTECTED"
      echo "!! These require a single-commit Bug Fix with explicit approval."
      FAIL=1
    fi
  done < "$PROTECTED_FILE"
fi

# ── 3. Scan changed files for hardcoded secrets ──────────────────────
# Focus on files that are most likely to contain secrets
SECRET_FILES=$(echo "$CHANGED_FILES" | grep -E '\.(cs|py|ts|js|php|config|env|yml|yaml|json|xml|ps1|sh|bat|ini|cpp|h|go|rb|java)$' || true)

if [ -n "$SECRET_FILES" ]; then
  echo "$SECRET_FILES" | while IFS= read -r file; do
    if [ ! -f "$file" ]; then continue; fi

    # Check for password/secret patterns
    if grep -qE 'password\s*[:=]\s*["'"'"']?[^"'"'"'\s,;]+' "$file" 2>/dev/null; then
      echo "!! SECURITY: Possible hardcoded password in $file"
      FAIL=1
    fi
    if grep -qE 'api[_-]?key\s*[:=]\s*["'"'"']?[^"'"'"'\s,;]{16,}' "$file" 2>/dev/null; then
      echo "!! SECURITY: Possible hardcoded API key in $file"
      FAIL=1
    fi
    if grep -qE 'token\s*[:=]\s*["'"'"']?[a-zA-Z0-9_-]{20,}' "$file" 2>/dev/null; then
      echo "!! SECURITY: Possible hardcoded token in $file"
      FAIL=1
    fi
    if grep -qE 'mongodb://.*:.*@|postgres://.*:.*@|mysql://.*:.*@' "$file" 2>/dev/null; then
      echo "!! SECURITY: Possible connection string with credentials in $file"
      FAIL=1
    fi
  done
fi

# ── 4. Summary & exit ────────────────────────────────────────────────
if [ "$FAIL" -eq 1 ]; then
  echo ""
  echo "!! Push BLOCKED. Fix the issues above and try again."
  echo "!! If you need to bypass (e.g. you are intentionally adding a sample),"
  echo "!! use: git push --no-verify"
  exit 1
fi

exit 0
