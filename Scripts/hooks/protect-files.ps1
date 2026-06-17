param([string]$FilePath)

# Support dual-mode: arg (Claude Code) or stdin JSON (OpenCode)
if (-not $FilePath) {
  try {
    $json = [Console]::In.ReadToEnd()
    if ($json) { $event = $json | ConvertFrom-Json; $FilePath = $event.tool_input.file_path }
    if (-not $FilePath) { $FilePath = $event.tool_input.filePath }
  } catch { }
}

$configPath = Join-Path $PSScriptRoot "protected-files.txt"
if (-not (Test-Path $configPath)) { exit 0 }
if (-not $FilePath) { exit 0 }

$FilePath = $FilePath.Replace('\', '/')
$patterns = Get-Content $configPath | Where-Object { $_ -and $_ -notmatch '^\s*#' -and $_.Trim() -ne '' }

foreach ($pattern in $patterns) {
  $pattern = $pattern.Trim()
  $regex = [Regex]::Escape($pattern) -replace '\\\*\*', '.*' -replace '\\\*', '[^/]*' -replace '\\\?', '.'
  if ($FilePath -match $regex) {
    [Console]::Error.WriteLine("!! PROTECT: $FilePath (matched: $pattern)")
    exit 2
  }
}

exit 0
