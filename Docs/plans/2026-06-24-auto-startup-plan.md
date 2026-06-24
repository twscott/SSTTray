# SSTTray + Shioaji 統一起動實作計畫

> **FOR AGENT:** Use superpowers:subagent-driven-development to implement this plan task-by-task.
> Or execute step by step in current session.

**Goal:** 建立 `start_ssttray.bat` 統一起動腳本，確保每天重開機/登入後 SSTTray 與 Shioaji Server 自動正常啟動。

**Architecture:** 單一批處理腳本（.bat），包含 Shioaji 啟動/健康檢查、SSTTray 啟動、背景監控迴圈三大階段。搭配 Windows 啟動資料夾捷徑與 Task Scheduler 作為觸發入口。

**Tech Stack:** Windows Batch Script, Shioaji HTTP API (port 8080), Windows Task Scheduler, Startup Folder

---

### Task 1: 建立 `start_ssttray.bat` — 統一起動腳本

**Files:**
- Create: `D:\ScottWork\SSTTray\start_ssttray.bat`

**Step 1: 建立腳本框架**

```batch
@echo off
setlocal enabledelayedexpansion

set SCRIPT_DIR=%~dp0
set SSTTRAY_EXE=%SCRIPT_DIR%SSTTray\bin\Release\SSTTray.exe
set LOG_FILE=%SCRIPT_DIR%start_ssttray.log
set SHIOAJI_URL=http://127.0.0.1:8080/api/v1/health
set MONITOR_INTERVAL=60
set SHIOAJI_RETRY_MAX=12
set SHIOAJI_RETRY_INTERVAL=5

:: 初始化順序鎖（防止多個實例同時執行階段 1）
set INIT_LOCK=%TEMP%\ssttray_init.lock

call :log "=== start_ssttray.bat 啟動 ==="
call :log "工作目錄: %SCRIPT_DIR%"
call :checkPrerequisites
```

**Step 2: 實作 log 函數**

```batch
:log
set TIMESTAMP=%DATE% %TIME%
echo [%TIMESTAMP%] %* >> "%LOG_FILE%"
echo [%TIMESTAMP%] %*
exit /b 0
```

**Step 3: 實作前置檢查（check java/python/shioaji 指令是否存在）**

```batch
:checkPrerequisites
where shioaji >nul 2>&1
if %ERRORLEVEL% neq 0 (
    call :log "[錯誤] shioaji 指令不在 PATH 中"
    call :log "請執行: pip install shioaji"
    exit /b 1
)
call :log "[OK] shioaji 指令已就緒"
exit /b 0
```

**Step 4: 實作 Shioaji 啟動與健康檢查**

```batch
:ensureShioajiServer
call :log "=== 階段 1: 檢查 Shioaji Server ==="
call :checkShioajiHealth
if %ERRORLEVEL% equ 0 (
    call :log "[OK] Shioaji Server 已在執行中 (port 8080)"
    exit /b 0
)

call :log "[啟動] Shioaji Server 不在，準備啟動..."
:: 使用 VBS 隱藏啟動避免 terminal 視窗
if exist "%SCRIPT_DIR%start_shioaji.vbs" (
    call :log "[啟動] 透過 start_shioaji.vbs 啟動 (隱藏模式)"
    wscript "%SCRIPT_DIR%start_shioaji.vbs" //nologo
) else if exist "%SCRIPT_DIR%start_shioaji.bat" (
    call :log "[啟動] 透過 start_shioaji.bat 啟動"
    start "" /B "%SCRIPT_DIR%start_shioaji.bat"
) else (
    call :log "[啟動] 直接執行 shioaji server start"
    start "" /B cmd /c "cd /d %SCRIPT_DIR% && shioaji server start --no-open"
)

:: 等 Shioaji Server 起來（最多 SHIOAJI_RETRY_MAX 次，每次間隔 SHIOAJI_RETRY_INTERVAL 秒）
call :log "[等待] 等待 Shioaji Server 啟動中..."
set RETRY_COUNT=0
:waitShioajiLoop
timeout /t %SHIOAJI_RETRY_INTERVAL% /nobreak >nul
set /a RETRY_COUNT+=1
call :checkShioajiHealth
if %ERRORLEVEL% equ 0 (
    call :log "[OK] Shioaji Server 已就緒 (嘗試 %RETRY_COUNT% 次)"
    exit /b 0
)
if %RETRY_COUNT% geq %SHIOAJI_RETRY_MAX% (
    call :log "[警告] Shioaji Server 啟動逾時 (%RETRY_COUNT% 次嘗試後仍未就緒)"
    exit /b 1
)
goto waitShioajiLoop
```

**Step 5: 實作 Shioaji Health Check 函數**

```batch
:checkShioajiHealth
:: 使用 curl 或 powershell 檢查 health endpoint
powershell -Command "try { $r = Invoke-WebRequest -Uri '%SHIOAJI_URL%' -UseBasicParsing -TimeoutSec 5; if ($r.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }" >nul 2>&1
if %ERRORLEVEL% equ 0 (
    exit /b 0
) else (
    exit /b 1
)
```

**Step 6: 實作 SSTTray 啟動**

```batch
:ensureSSTTray
call :log "=== 階段 2: 檢查 SSTTray ==="
:: 檢查 SSTTray 是否已在執行
tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I /N "SSTTray.exe" >nul
if %ERRORLEVEL% equ 0 (
    call :log "[OK] SSTTray.exe 已在執行中"
    exit /b 0
)

:: 檢查 exe 是否存在
if not exist "%SSTTRAY_EXE%" (
    call :log "[錯誤] SSTTray.exe 不存在: %SSTTRAY_EXE%"
    exit /b 1
)

call :log "[啟動] 啟動 SSTTray.exe..."
start "" /D "%SCRIPT_DIR%SSTTray\bin\Release" "%SSTTRAY_EXE%"
timeout /t 3 /nobreak >nul

:: 驗證 SSTTray 確實啟動
tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I /N "SSTTray.exe" >nul
if %ERRORLEVEL% equ 0 (
    call :log "[OK] SSTTray.exe 已成功啟動"
    exit /b 0
) else (
    call :log "[錯誤] SSTTray.exe 啟動後未出現在處理程序清單中"
    exit /b 1
)
```

**Step 7: 實作背景監控迴圈**

```batch
:monitorLoop
call :log "=== 階段 3: 進入背景監控模式 ==="
call :log "每 %MONITOR_INTERVAL% 秒檢查一次"

:monitorLoopStart
timeout /t %MONITOR_INTERVAL% /nobreak >nul

:: 檢查 SSTTray
tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I /N "SSTTray.exe" >nul
if %ERRORLEVEL% neq 0 (
    call :log "[監控] SSTTray.exe 不在執行中，重新啟動..."
    start "" /D "%SCRIPT_DIR%SSTTray\bin\Release" "%SSTTRAY_EXE%"
    timeout /t 3 /nobreak >nul
    tasklist /FI "IMAGENAME eq SSTTray.exe" 2>nul | find /I /N "SSTTray.exe" >nul
    if %ERRORLEVEL% equ 0 (
        call :log "[監控] SSTTray.exe 重新啟動成功"
    ) else (
        call :log "[監控] SSTTray.exe 重新啟動失敗"
    )
)

:: 檢查 Shioaji Server
call :checkShioajiHealth
if %ERRORLEVEL% neq 0 (
    call :log "[監控] Shioaji Server 不在，重新啟動..."
    wscript "%SCRIPT_DIR%start_shioaji.vbs" //nologo 2>nul
    if exist "%SCRIPT_DIR%start_shioaji.bat" (
        start "" /B "%SCRIPT_DIR%start_shioaji.bat"
    )
    goto monitorLoopStart
)

goto monitorLoopStart
```

**Step 8: 組裝主流程**

```batch
:: ===== 主流程 =====

:: 使用互斥鎖確保階段 1+2 只執行一次
:: （多個觸發來源同時執行時不會重複啟動）
if exist "%INIT_LOCK%" (
    call :log "[資訊] 初始化鎖存在，略過階段 1+2（另一個實例已處理）"
    goto monitorLoop
)

:: 建立鎖檔案
echo %DATE% %TIME% > "%INIT_LOCK%"
call :ensureShioajiServer
call :ensureSSTTray

:: 背景監控（保持腳本活著）
:monitorLoop
call :monitorLoop
```

**Step 9: 最終腳本組合 — 整合以上所有片段**

腳本儲存於 `D:\ScottWork\SSTTray\start_ssttray.bat`。

**驗證方式：**
```bash
cd D:\ScottWork\SSTTray
start_ssttray.bat
```
預期結果：
- Shioaji Server 在 port 8080 上啟動
- SSTTray.exe 在系統列出現
- `start_ssttray.log` 有啟動記錄

---

### Task 2: 建立啟動資料夾捷徑

**Files:**
- Create: `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup\SSTTray.lnk`

**Step 1: 使用 PowerShell 建立捷徑**

```powershell
$WshShell = New-Object -ComObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Startup\SSTTray.lnk")
$Shortcut.TargetPath = "D:\ScottWork\SSTTray\start_ssttray.bat"
$Shortcut.WorkingDirectory = "D:\ScottWork\SSTTray"
$Shortcut.WindowStyle = 7  # Minimized
$Shortcut.Save()
```

**驗證方式：**
```bash
ls "$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Startup\SSTTray.lnk"
```

**Step 2: 備份原有 ShioajiServer.lnk**

```bash
Rename-Item "$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Startup\ShioajiServer.lnk" "ShioajiServer.lnk.bak"
```

---

### Task 3: 更新 Task Scheduler

**Files:**
- Modify: Task Scheduler `\SSTTray Monitor`

**Step 1: 查出現有任務設定**
```bash
schtasks /query /tn "SSTTray Monitor" /xml
```

**Step 2: 修改任務動作**
```bash
schtasks /change /tn "SSTTray Monitor" /tr "D:\ScottWork\SSTTray\start_ssttray.bat"
```

**Step 3: 驗證任務設定**
```bash
schtasks /query /tn "SSTTray Monitor" /v /fo LIST
```
確認 `任務執行程式` 欄位顯示為 `D:\ScottWork\SSTTray\start_ssttray.bat`

---

### Task 4: 完整驗證

**Step 1: 手動執行一次**（重開機前）
```bash
D:\ScottWork\SSTTray\start_ssttray.bat
```

**Step 2: 驗證啟動結果**
```bash
:: 檢查 SSTTray 是否執行
tasklist /FI "IMAGENAME eq SSTTray.exe"

:: 檢查 Shioaji 是否就緒
powershell -Command "Invoke-WebRequest -Uri http://127.0.0.1:8080/api/v1/health -UseBasicParsing"

:: 檢查啟動紀錄
type D:\ScottWork\SSTTray\start_ssttray.log
```

**Step 3: 測試監控重啟**
```bash
:: 手動關閉 SSTTray（從系統列 Exit）
:: 等待 60 秒後確認是否自動重啟
```

**Step 4: 測試重開機情境**（最後一步）
請使用者下次重開機後確認：
- 08:00 登入後系統列出現 SSTTray 圖示
- Shioaji Server 正常運作（/health 回 200）
- alertlog 有資料
