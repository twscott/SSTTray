# ============================================================
# protect-reference-code.ps1
# PreToolUse Hook: Windows 版
# ============================================================

param(
    [string]$FilePath
)

$referencePaths = @(
    "\\*\share\legacy\*"
    "D:\legacy-systems\*"
)

foreach ($pattern in $referencePaths) {
    if ($FilePath -like $pattern) {
        Write-Host ""
        Write-Host "⛔ BLOCKED: Reference code is READ-ONLY"
        Write-Host "   File: $FilePath"
        Write-Host "   This path is for reference only. Modification is not allowed."
        Write-Host ""
        Write-Host "   If this is a legitimate exception, you must:"
        Write-Host "   1. Contact the user to confirm"
        Write-Host "   2. Have user temporarily disable this hook"
        Write-Host ""
        exit 2
    }
}

exit 0
