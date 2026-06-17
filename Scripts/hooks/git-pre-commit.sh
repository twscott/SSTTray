#!/usr/bin/env bash
# git pre-commit hook — universal fallback for all AI tools
# Runs protect-files + scan-hardcoded on staged files before commit

HOOKS_DIR="$(git rev-parse --show-toplevel)/scripts/hooks"

if [[ ! -d "$HOOKS_DIR" ]]; then
  exit 0
fi

staged_files=$(git diff --cached --name-only --diff-filter=ACM)
[[ -z "$staged_files" ]] && exit 0

has_critical=0

while IFS= read -r file; do
  if [[ -f "$HOOKS_DIR/protect-files.ps1" ]]; then
    echo "{\"tool_input\":{\"filePath\":\"$file\"}}" | powershell -NoProfile -File "$HOOKS_DIR/protect-files.ps1"
    if [[ $? -eq 2 ]]; then
      has_critical=1
    fi
  fi

  if [[ -f "$HOOKS_DIR/scan-hardcoded.ps1" ]]; then
    echo "{\"tool_input\":{\"filePath\":\"$file\"}}" | powershell -NoProfile -File "$HOOKS_DIR/scan-hardcoded.ps1"
  fi
done <<< "$staged_files"

if [[ $has_critical -eq 1 ]]; then
  echo "" >&2
  echo "!! Commit BLOCKED: contains protected file modifications." >&2
  echo "If Bug Fix, commit separately with explanation." >&2
  exit 1
fi

# Check for missing agent entry files
repo_root=$(git rev-parse --show-toplevel)
if [[ ! -f "$repo_root/AGENTS.md" ]] && [[ ! -f "$repo_root/CLAUDE.md" ]] && [[ ! -f "$repo_root/.cursorrules" ]]; then
  echo "Warning: No AI agent entry file found (AGENTS.md / CLAUDE.md / .cursorrules)." >&2
  echo "  Run: .\\scripts\\setup-agent.ps1 -ProjectPath . -Agent all" >&2
fi

exit 0
