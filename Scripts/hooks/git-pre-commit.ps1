<#
.SYNOPSIS
    Git pre-commit hook: universal fallback for AI agent instruction enforcement.
.DESCRIPTION
    Runs on every git commit regardless of which tool is used.
    - Checks staged files against protected-files.txt
    - Scans staged files for hardcoded secrets
    Does NOT check production DB (that's tool-specific).
    Exits 1 if critical violation found, 0 otherwise.
#>

$hookDir = Split-Path $PSScriptRoot -Parent
$protectedFilesConfig = Join-Path $hookDir "protected-files.txt"
$blockedCount = 0
$warningsCount = 0

# Get staged files
$stagedFiles = git diff --cached --name-only --diff-filter=ACM
if (-not $stagedFiles) {
    exit 0
}

# --- Check 1: Protected files ---
if (Test-Path $protectedFilesConfig) {
    $patterns = Get-Content $protectedFilesConfig | Where-Object {
        $_ -and $_ -notmatch '^\s*#' -and $_.Trim() -ne ''
    }

    foreach ($file in $stagedFiles) {
        foreach ($pattern in $patterns) {
            $pattern = $pattern.Trim()
            $regex = [Regex]::Escape($pattern) `
                -replace '\\\*\*', '.*' `
                -replace '\\\*', '[^/]*' `
                -replace '\\\?', '.'

            if ($file -match $regex) {
                Write-Host "⛔ BLOCKED: Protected file in commit: $file (matched: $pattern)"
                $blockedCount++
            }
        }
    }
}

# --- Check 2: Hardcoded secrets scan ---
foreach ($file in $stagedFiles) {
    if (-not (Test-Path $file)) { continue }

    $content = Get-Content $file -Raw -ErrorAction SilentlyContinue
    if (-not $content) { continue }

    if ($content -match '(?i)(password|pwd|passwd|api[_-]?key|secret[_-]?key)\s*[:=]\s*["''][^$][^"'']+["'']') {
        Write-Host "⚠️  WARNING: Possible hardcoded secret in: $file"
        $warningsCount++
    }
}

# --- Result ---
if ($blockedCount -gt 0) {
    Write-Host "`n❌ Commit blocked: $blockedCount protected file(s) detected."
    Write-Host "   Remove them from the commit or update protected-files.txt."
    exit 1
}

if ($warningsCount -gt 0) {
    Write-Host "`n⚠️  $warningsCount file(s) with possible hardcoded secrets."
    Write-Host "   Review before pushing."
}

exit 0
