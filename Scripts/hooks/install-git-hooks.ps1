<#
.SYNOPSIS
  Install git hooks (pre-commit + pre-push) into the current project.
.DESCRIPTION
  Copies scripts from scripts/hooks/ to .git/hooks/.
  Run from project root: .\scripts\hooks\install-git-hooks.ps1
#>

$gitDir = git rev-parse --git-dir 2>$null
if (-not $gitDir) {
  Write-Error "Not a git repository."
  exit 1
}

$hooksDir = Join-Path $gitDir "hooks"
if (-not (Test-Path $hooksDir)) {
  New-Item -ItemType Directory -Path $hooksDir -Force | Out-Null
}

$hooks = @(
  @{ Source = "git-pre-commit.sh"; Dest = "pre-commit" }
  @{ Source = "git-pre-push.sh";   Dest = "pre-push" }
)

foreach ($hook in $hooks) {
  $source = Join-Path $PSScriptRoot $hook.Source
  $dest = Join-Path $hooksDir $hook.Dest

  if (-not (Test-Path $source)) {
    Write-Warning "Skip: $($hook.Source) not found"
    continue
  }

  Copy-Item -Path $source -Destination $dest -Force
  Write-Host "  [+] Installed $($hook.Dest)" -ForegroundColor Green
}

Write-Host "`nGit hooks installed to $hooksDir"
Write-Host "  - pre-commit: runs on every 'git commit'"
Write-Host "  - pre-push:   runs on every 'git push' (checks secrets + protected files)"
