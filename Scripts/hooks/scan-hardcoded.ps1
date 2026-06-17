param([string]$FilePath)

# Support dual-mode: arg (Claude Code) or stdin JSON (OpenCode)
if (-not $FilePath) {
  try {
    $json = [Console]::In.ReadToEnd()
    if ($json) { $event = $json | ConvertFrom-Json; $FilePath = $event.tool_input.file_path }
    if (-not $FilePath) { $FilePath = $event.tool_input.filePath }
  } catch { }
}

if (-not $FilePath -or -not (Test-Path $FilePath)) { exit 0 }

$content = Get-Content $FilePath -Raw -ErrorAction SilentlyContinue
if (-not $content) { exit 0 }

$issues = @()
if ($content -match '(?i)(password|pwd|passwd)\s*[:=]\s*["''](?![$({])([^"'']+)["'']') { $issues += "!! Possible hardcoded password" }
if ($content -match '(?i)(api[_-]?key|apikey|secret[_-]?key|secretkey|token)\s*[:=]\s*["''](?![$({])([^"'']{8,})["'']') { $issues += "!! Possible API key or token" }
if ($content -match '(?i)(connection\s*string|connstr)\s*[:=]\s*["''](.*?password\s*=\s*[^;]+)["'']') { $issues += "!! Connection string with embedded password" }
if ($content -match '(?i)[A-Z]:\\[^"''\n]*\\(Documents|Users|Temp|AppData|Desktop)\\[^"''\n]*') { $issues += "!! Hardcoded local path" }

if ($issues.Count -gt 0) {
  Write-Host "== SCAN [$FilePath] =="
  $issues | ForEach-Object { Write-Host $_ }
}

exit 0
