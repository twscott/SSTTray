<#
.SYNOPSIS
  sstTray deployment secret env-var provisioning / check script (NO values stored)

.DESCRIPTION
  Per Docs/SECRETS.md, writes secret values into the CURRENT USER env scope
  (or -Machine with admin). Values come only from command-line parameters -
  never stored in this file or in the repo. -Check only inspects.

.PARAMETER Check        Only check and report each var (values never printed).
.PARAMETER Machine      Write to Machine scope (requires admin). Default: User.
.PARAMETER FailOnMissing  Exit 1 if any listed var is missing at the end.

.EXAMPLE
  .\set-secrets.ps1 -Check
  .\set-secrets.ps1 -SST_DB_PWD '...' -SHIOAJI_API_KEY '...' -SHIOAJI_SECRET_KEY '...'
  .\set-secrets.ps1 -Check -FailOnMissing
#>
param(
  [switch]$Check,
  [switch]$Machine,
  [switch]$FailOnMissing,
  [string]$SST_DB_PWD,
  [string]$SST_DB_DIGGO_PWD,
  [string]$SST_LINE_APP_KEY,
  [string]$SST_LINE_KEY,
  [string]$SST_BAK_KEY_A,
  [string]$SST_BAK_KEY_B,
  [string]$SST_BAK_KEY_C,
  [string]$SST_BAK_KEY_D,
  [string]$SST_BAK_KEY_E,
  [string]$SST_SMTP_PWD,
  [string]$SST_FTP_USER,
  [string]$SST_FTP_PWD,
  [string]$SHIOAJI_API_KEY,
  [string]$SHIOAJI_SECRET_KEY
)

$ErrorActionPreference = "Stop"
$scope = if ($Machine) { 'Machine' } else { 'User' }

$names = @(
  'SST_DB_PWD','SST_DB_DIGGO_PWD','SST_LINE_APP_KEY','SST_LINE_KEY',
  'SST_BAK_KEY_A','SST_BAK_KEY_B','SST_BAK_KEY_C','SST_BAK_KEY_D','SST_BAK_KEY_E',
  'SST_SMTP_PWD','SST_FTP_USER','SST_FTP_PWD','SHIOAJI_API_KEY','SHIOAJI_SECRET_KEY'
)

Write-Host ("sstTray secret env vars (scope={0})" -f $scope) -ForegroundColor Cyan
$missing = @()
foreach ($name in ($names | Sort-Object)) {
  $cur = [Environment]::GetEnvironmentVariable($name, $scope)
  $v = Get-Variable -Name $name -ErrorAction SilentlyContinue
  $provided = ($null -ne $v) -and (-not [string]::IsNullOrEmpty($v.Value))
  if ($provided) {
    [Environment]::SetEnvironmentVariable($name, $v.Value, $scope)
    Write-Host ("  {0,-22} = [SET] (scope={1})" -f $name, $scope) -ForegroundColor Green
  } elseif ($cur) {
    Write-Host ("  {0,-22} = [already set]" -f $name) -ForegroundColor Green
  } else {
    $missing += $name
    Write-Host ("  {0,-22} = [MISSING]" -f $name) -ForegroundColor Yellow
  }
}

if ($missing.Count -gt 0) {
  Write-Host ("MISSING {0}: {1}" -f $missing.Count, ($missing -join ', ')) -ForegroundColor Yellow
  if ($FailOnMissing) { Write-Host 'FAIL: not all secrets provisioned.' -ForegroundColor Red; exit 1 }
} else {
  Write-Host 'OK: all secret env vars are present.' -ForegroundColor Green
}
exit 0