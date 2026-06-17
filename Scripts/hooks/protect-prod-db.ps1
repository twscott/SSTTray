param([string]$Command)

# Support dual-mode: arg (Claude Code) or stdin JSON (OpenCode)
if (-not $Command) {
  try {
    $json = [Console]::In.ReadToEnd()
    if ($json) { $event = $json | ConvertFrom-Json; $Command = $event.tool_input.command }
    if (-not $Command) { $Command = $event.tool_input.cmd }
  } catch { }
}

$configPath = Join-Path $PSScriptRoot "prod-db-hosts.txt"
if (-not (Test-Path $configPath)) { exit 0 }
if (-not $Command) { exit 0 }

$prodHosts = Get-Content $configPath | Where-Object { $_ -and $_ -notmatch '^\s*#' -and $_.Trim() -ne '' }
$dangerousOps = @('INSERT\s+INTO', 'UPDATE\s+', 'DELETE\s+FROM', 'DROP\s+', 'ALTER\s+', 'TRUNCATE\s+')

foreach ($hostPattern in $prodHosts) {
  $hostPattern = $hostPattern.Trim()
  if ($hostPattern -eq '') { continue }

  if ($Command -match [Regex]::Escape($hostPattern)) {
    $isSelectOnly = ($Command -match '(?i)\bSELECT\b') -and (-not ($dangerousOps | Where-Object { $Command -match "(?i)$_" }))
    if (-not $isSelectOnly) {
      [Console]::Error.WriteLine("!! PROD-DB: blocked $hostPattern")
      exit 2
    }
  }
}

exit 0
