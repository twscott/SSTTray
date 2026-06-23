# 維護記錄 — SSTTray

## 2026-06-23 — Session #4：Shioaji Server 自動啟動修復 & alertlog 恢復

**完成項目**：
- 診斷 SSTTray alertlog 自 2026-05 起無新資料的根本原因
- 發現 Shioaji server (port 8080) 未自動啟動，導致 `shioajiStockAlert()` 跳過處理
- 手動啟動 Shioaji server，alertlog 恢復寫入（10:50 起已有資料）
- 修復 `sst.cs` 中 `shioajiLogin()` 的 auto-start 腳本路徑搜尋邏輯（增加 `rootDir` 層級）
- 修復 `D:\vibeCoding\sst\src\` 的 SST.StockImport.API 兩個 startup 崩潰問題：
  - `ServerVersion.AutoDetect()` → `ServerVersion.Parse("8.0.31-mysql")`
  - `SetCurrentDirectory` / `ContentRootPath` 移到 `CreateBuilder` 之前
- 建立 `install_shioaji_task.bat` 供建立 Windows 排程（每天早上 8:00 自動啟 Shioaji）

**產出文件**：
- `SSTTray/sst.cs`（修改 — shioajiLogin 路徑修正）
- `Docs/Maintenance/CURRENT_HANDOFF.md`（新建 — 交接檔）
- `../install_shioaji_task.bat`（新建 — 排程安裝批次檔）

**外部系統修改**：
- `D:\vibeCoding\sst\src\SST.StockImport.Infrastructure\ServiceCollectionExtensions.cs`（ServerVersion 修正）
- `D:\vibeCoding\sst\src\SST.StockImport.API\Program.cs`（SetCurrentDirectory 時序修正）

**下次繼續**：
- 若使用者尚未執行 `install_shioaji_task.bat`，請以系統管理員身分執行以建立 Shioaji 每日自動啟動排程
- SSTTray 若需完整 rebuild，需以 Visual Studio Developer Command Prompt 編譯
