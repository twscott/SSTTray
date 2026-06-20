# 維護記錄 — SSTTray

> **建立日期**: 2026-06-17

---

## 維護歷史

### 2026-06-17 — 系統首次接入（First Apply）+ Shioaji Token 外移

**完成項目**：
- 從 companyMaintains 倉庫取得最新維護規範並套用
- 建立 SYSTEM-PROFILE.md（系統基本資料）
- 建立 AGENTS.md（核心策略層）
- 安裝 scripts/hooks/ 保護腳本 + Git Hooks
- 更新 .gitignore 保護 SYSTEM-PROFILE.md
- 更新 copilot-instructions.md
- 將 Shioaji API Key/Secret 從 sst.cs 硬編碼移至 Windows 使用者環境變數

**產出文件**：
- `Docs/SYSTEM-PROFILE.md`（新建）
- `AGENTS.md`（新建）
- `scripts/hooks/`（新建）
- `Docs/Maintenance/MAINTENANCE-LOG.md`（新建）
- `SSTTray/.github/copilot-instructions.md`（更新）
- `.gitignore`（更新）
- `SSTTray/sst.cs`（修改 - line 939 改為讀取環境變數）

**Gate 狀態**：未觸達（本次為首次接入 + 小修改，非完整維護週期）

**下次繼續**：無待辦事項。下次開工時由工程師提出新維護需求。

### 2026-06-19 — Shioaji HTTP API 遷移 + 程式碼重構 + 通知機制追加

**完成項目**：
- Shioaji 認證修復：舊 Sinopac.Shioaji C# SDK → Shioaji HTTP API
- `shioajiStockAlert()` 改為 HttpClient POST /api/v1/data/snapshots
- `stockDictToMsgArray()` datetime 格式相容處理
- `shioajiLogin()` 改為 localhost:8080 health check + 自動啟動 server + CA 憑證到期檢查
- `detector()` 5 組重複 try-catch 抽成 `detectorErrHandler()` 共用錯誤處理 + writeLog
- `processAlert()` 5 組重複 skip 驗證抽成 `skipStock()` helper
- `processAlert()` 清理約 15 個未使用的變數宣告
- 新增可疑 volume 資料 logging（累計量倒退、panVol<=0、極高倍率）
- 新增每日 13:35 資料健康檢查（alertlog 0 筆→寄信，<100 筆→寫 log）
- 新增 CA 憑證到期前 30 天 Line 通知
- 新增 `shioajiLogin()` 失敗時 Line 通知
- 新增開機自動啟動設定（Windows Startup + start_shioaji.bat）
- `start_shioaji.bat` 建立與開機捷徑
- `.gitignore` 新增 .env / shioaji_server 日誌

**產出文件**：
- `SSTTray/sst.cs`（修改 - 核心遷移 + logging）
- `SSTTray/TaskTrayApplicationContext.cs`（修改 - detector 重構 + 每日檢查）
- `SSTTray/SSTTray.csproj`（修改 - 移除 Sinopac.Shioaji NuGet）
- `docs/plans/2026-06-19-shioaji-http-migration.md`（新建 - 實作計畫）
- `start_shioaji.bat`（新建 - 開機啟動腳本）
- `.gitignore`（更新 - 新增 .env / server log）

**Gate 狀態**：未觸達（非完整維護週期）

**下次繼續**：
- 觀察下個交易日 alertlist 資料是否正常收集
- 如 FirstohmService 需要更新監視路徑，後續處理

### 2026-06-20 — detector() + insertAlertList() 重構 + Shioaji 隱藏啟動

**完成項目**：
- `insertAlertList()` pav direction=0 bug fix（panVol5CntPos → panVol5CntNeg）
- `insertAlertList()` if-else 40 行 → AlertTypeMap Dictionary 查找表
- 新增 `calcPanLevel()` 統一主進主出判斷，processAlert() 改 switch
- `detector()` 移除 4 個無意義 `CommonClass.wait(1)`
- `start_shioaji.vbs` 建立，Shioaji server 完全隱藏啟動（無 terminal）
- 開機捷徑從 .bat 改指向 .vbs
- `shioajiLogin()` 自動啟動優先嘗試 .vbs

**產出文件**：
- `start_shioaji.vbs`（新建）
- `SSTTray/Docs/plans/2026-06-20-detector-insertAlertList-refactor-design.md`（新建）
- `SSTTray/Docs/plans/2026-06-20-detector-insertAlertList-refactor-plan.md`（新建）
- `SSTTray/sst.cs`（修改）
- `SSTTray/TaskTrayApplicationContext.cs`（修改）
- `start_shioaji.bat`（同一份，無修改）

**Gate 狀態**：未觸達（重構維護週期）

**下次繼續**：
- 無待辦事項。下次開工時由工程師提出新維護需求。
