@echo off
setlocal enabledelayedexpansion

set SCRIPT_DIR=%~dp0
set SSTTRAY_DIR=%SCRIPT_DIR%SSTTRAY\bin\Release
set SSTTRAY_EXE=%SSTTRAY_DIR%\SSTTray.exe
set LOG_FILE=%SCRIPT_DIR%start_ssttray.log
set SHIOAJI_URL=http://127.0.0.1:8080/api/v1/health
set SHIOAJI_RETRY_MAX=12
set SHIOAJI_RETRY_INTERVAL=5
set MONITOR_INTERVAL=60

set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% ====== >> "%LOG_FILE%"
echo %TIMESTAMP% start_ssttray.bat starting >> "%LOG_FILE%"
echo %TIMESTAMP% ScriptDir: %SCRIPT_DIR% >> "%LOG_FILE%"

:phase0
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Phase0: checks >> "%LOG_FILE%"
if not exist "%SSTTRAY_EXE%" (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% WARN: exe not found: %SSTTRAY_EXE% >> "%LOG_FILE%"
)
where shioaji >nul 2>&1
if %ERRORLEVEL% neq 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% WARN: shioaji not in PATH >> "%LOG_FILE%"
)

:phase1
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Phase1: Shioaji Server >> "%LOG_FILE%"
call :checkShioajiHealth
if %ERRORLEVEL% equ 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% Shioaji already running >> "%LOG_FILE%"
    goto phase2
)
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Shioaji not running, starting... >> "%LOG_FILE%"

start "" /B cmd /c "cd /d %SCRIPT_DIR% && shioaji server start --no-open"
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Started direct: shioaji server start >> "%LOG_FILE%"

if not exist "%SCRIPT_DIR%start_shioaji.bat" (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% (fallback: start_shioaji.bat not needed, direct approach succeeded) >> "%LOG_FILE%"
)

call :waitShioajiReady

:phase2
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Phase2: SSTTray >> "%LOG_FILE%"
tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I "SSTTray.exe" >nul
if %ERRORLEVEL% equ 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% SSTTray already running >> "%LOG_FILE%"
    goto monitor
)
if not exist "%SSTTRAY_EXE%" (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% SSTTray.exe not found >> "%LOG_FILE%"
    goto monitor
)
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Starting SSTTray.exe... >> "%LOG_FILE%"
start "" /D "%SSTTRAY_DIR%" "%SSTTRAY_EXE%"
timeout /t 5 /nobreak >nul 2>&1
tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I "SSTTray.exe" >nul
if %ERRORLEVEL% equ 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% SSTTray started OK >> "%LOG_FILE%"
) else (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% SSTTray not started yet >> "%LOG_FILE%"
)

:monitor
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Monitor mode every %MONITOR_INTERVAL% sec >> "%LOG_FILE%"

:monitorStart
timeout /t %MONITOR_INTERVAL% /nobreak >nul 2>&1

tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I "SSTTray.exe" >nul
if %ERRORLEVEL% neq 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% MON: SSTTray down, restarting >> "%LOG_FILE%"
    if exist "%SSTTRAY_EXE%" (start "" /D "%SSTTRAY_DIR%" "%SSTTRAY_EXE%")
)

call :checkShioajiHealth
if %ERRORLEVEL% neq 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% MON: Shioaji down, restarting >> "%LOG_FILE%"
    if exist "%SCRIPT_DIR%start_shioaji.vbs" (start "" wscript "%SCRIPT_DIR%start_shioaji.vbs" //nologo)
    call :waitShioajiReady
)
goto monitorStart

:waitShioajiReady
set RETRY=0
:waitLoop
timeout /t %SHIOAJI_RETRY_INTERVAL% /nobreak >nul 2>&1
set /a RETRY+=1
call :checkShioajiHealth
if %ERRORLEVEL% equ 0 (
    set TIMESTAMP=%DATE% %TIME%
    echo %TIMESTAMP% Shioaji ready after %RETRY% tries >> "%LOG_FILE%"
    exit /b 0
)
if %RETRY% lss %SHIOAJI_RETRY_MAX% goto waitLoop
set TIMESTAMP=%DATE% %TIME%
echo %TIMESTAMP% Shioaji startup timeout >> "%LOG_FILE%"
exit /b 1

:checkShioajiHealth
powershell -Command "try {$r=Invoke-WebRequest -Uri '%SHIOAJI_URL%' -UseBasicParsing -TimeoutSec 5;if($r.StatusCode -eq 200){exit 0}else{exit 1}}catch{exit 1}" >nul 2>&1
if %ERRORLEVEL% equ 0 (exit /b 0) else (exit /b 1)
