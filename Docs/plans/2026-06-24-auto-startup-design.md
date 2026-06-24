# Auto-Startup Design (v2) — SSTTray + Shioaji 統一起動

> **建立日期**: 2026-06-24  
> **狀態**: 設計已核准，待實作  
> **更新重點**: 系統 6:00 重開機後不須登入即可啟動 Shioaji Server

---

## 問題

電腦每天 06:00 自動重開機，約 10:00 才有使用者登入。SSTTray.exe 與 Shioaji Server 目前都依賴使用者登入才能啟動。

### 根因

| 問題 | 原因 |
|------|------|
| Shioaji Server 未自啟 | 啟動資料夾 + Task Scheduler 皆設 `InteractiveToken`，須使用者登入 |
| SSTTray.exe 未自啟 | 啟動資料夾無捷徑；Task Scheduler 亦須登入 |
| 6:00~10:00 空白 4 小時 | 重開機後無互動工作階段，Shioaji 應在此時就緒但無法啟動 |

---

## 解決方案：雙層啟動架構

### 架構

```
06:00 重開機
      │
      ▼
┌────────────────────────────────────┐
│ Task Scheduler（RunWhetherUserLoggedOn）│  ← 系統層，不須登入
│  啟動 start_ssttray.bat            │
└──────────┬─────────────────────────┘
           │
           ├── Shioaji Server (port 8080)  ✅ 啟動（無須 UI）
           ├── SSTTray.exe  ❌ 無互動工作階段無法啟動（NotifyIcon）
           └── 監控迴圈：每分鐘檢查 Shioaji，持續等待 SSTTray 啟動時機

      ...
10:00 使用者登入
      │
      ▼
┌────────────────────────────┐
│ 啟動資料夾 SSTTray.lnk     │  ← 使用者層
│  啟動 start_ssttray.bat    │
└──────────┬─────────────────┘
           │
           ├── Shioaji Server  ✅ 已執行中（跳過啟動）
           ├── SSTTray.exe    ✅ 現在有工作階段了，啟動成功
           └── 監控迴圈：每分鐘檢查兩者，掛了重啟
```

### 為什麼同一個 .bat 可以同時處理兩種情境？

`start_ssttray.bat` 設計為**完全等冪**：

| 情境 | Shioaji | SSTTray | 結果 |
|------|---------|---------|------|
| 6:00 系統啟動時 | 不在 → 啟動 ✅ | 不在但無法啟動 (no session) → 略過，稍後重試 | Shioaji ✅，SSTTray 持續等待 |
| 10:00 使用者登入時 | 已執行 → 跳過 ✅ | 不在 → 啟動 ✅ | 兩者都 ✅ |
| 使用者登入（SSTTray 已由監控啟動） | 已執行 → 跳過 | 已執行 → 跳過 | 不重複啟動 |

---

## 檔案變更

| 檔案 | 動作 | 說明 |
|------|------|------|
| `start_ssttray.bat` | **新建** | 統一起動腳本（等冪設計） |
| `start_ssttray.log` | **新建**（自動產生） | 執行日誌 |
| 啟動資料夾 `SSTTray.lnk` | **新建** | 指向 start_ssttray.bat |
| 啟動資料夾 `ShioajiServer.lnk` | **更名→.bak** | 備援保留 |
| Task Scheduler「SSTTray Monitor」| **修改** | 動作 → start_ssttray.bat；改為 RunWhetherUserLoggedOn |

---

## `start_ssttray.bat` 設計規格

### 階段 1：Shioaji Server 確保
- 檢查 port 8080 `/api/v1/health`
- 不在 → 執行 `start_shioaji.vbs` 隱藏啟動
- 等待 health 200（最長 60 秒，每 5 秒重試）
- 逾時 → 寫 log，不阻斷後續流程

### 階段 2：SSTTray 確保
- 檢查 `SSTTray.exe` 處理程序
- 不在且可啟動 → 啟動 SSTTray.exe
- 不在但無法啟動（無互動工作階段）→ 寫 log，稍後監控迴圈重試

### 階段 3：背景監控（每 60 秒）
- SSTTray 掛了 → 重啟
- Shioaji 掛了 → 重啟 + 等待 health
- 寫入 start_ssttray.log

### 等冪保護
- 初始化互斥鎖 `%TEMP%\ssttray_init.lock`
- 同時間多個觸發來源（Task Scheduler + 啟動資料夾）不會重複執行階段 1+2
- 監控迴圈無鎖，可多 instance 共存（僅重啟，不重複啟動）

---

## 不修改的檔案

- `sst.cs` — 不修改
- `GlobalConst.cs` — 保護檔案不動
- `start_shioaji.vbs` / `start_shioaji.bat` — 保留原樣

---

## 驗收標準

1. [ ] 重開機後不須登入，Shioaji Server 自動啟動（port 8080 /health 200）
2. [ ] 使用者登入後 2 分鐘內，系統列出現 SSTTray 圖示
3. [ ] 手動關閉 SSTTray 後，60 秒內自動重啟
4. [ ] 手動停止 Shioaji Server 後，60 秒內自動重啟
5. [ ] `start_ssttray.log` 有完整啟動/監控時間戳記錄
6. [ ] 收盤後可查閱 alertlog 當日有資料
