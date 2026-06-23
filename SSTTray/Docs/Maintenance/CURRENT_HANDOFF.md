# CURRENT_HANDOFF — SSTTray / Shioaji Server 維運

**Session 編號**：#4  
**收工時間**：2026-06-23 11:05  
**收工工程師**：Sisyphus

---

## 📍 當前位置

| 欄位 | 內容 |
|------|------|
| **系統** | SSTTray (System Tray App) |
| **本次工作** | Shioaji Server 自動啟動修復 & alertlog 恢復 |
| **Git 分支** | `master` |
| **當前階段** | 完成 — 等待使用者確認 Shioaji 排程建立 |

---

## ✅ 本次 Session 完成的事項

1. **診斷 alertlog 無資料問題** — 發現 Shioaji server (port 8080) 自 2026-06-19 起未執行，`shioajiStockAlert()` 因 `shioajiLogin()` 失敗而跳過處理
2. **手動啟動 Shioaji server** — PID 18196，alertlog 已於 10:50 恢復寫入
3. **修復 `sst.cs` auto-start 路徑** — `shioajiLogin()` 尋找 `start_shioaji.vbs`/`.bat` 時增加 `rootDir` 層級，正確指向 `D:\ScottWork\SSTTray\`
4. **修復 SST.StockImport.API startup 崩潰** — 兩個問題：
   - `ServerVersion.AutoDetect` → `ServerVersion.Parse("8.0.31-mysql")`
   - `SetCurrentDirectory`/`ContentRootPath` 移到 `CreateBuilder` 之前
5. **建立 `install_shioaji_task.bat`** — 供管理員一鍵建立 Shioaji 每日 08:00 自動啟動排程

---

## ⏭️ 下次開工第一件事

**操作**：確認 Shioaji server 是否在運作，檢查 alertlog 是否有今日資料

**命令**：
```sql
SELECT COUNT(*), MAX(CREATED) FROM alertlog WHERE CREATED >= CURDATE();
```

---

## ⚠️ 等待工程師處理的事項

- [ ] **建立 Shioaji 排程工作**：以**系統管理員**身分執行 `D:\ScottWork\SSTTray\install_shioaji_task.bat`
      或手動執行：
      ```
      schtasks /create /tn "Shioaji Server" /tr "C:\Users\Scott.Tseng\AppData\Roaming\Python\Python311\Scripts\shioaji.exe server start --production" /sc daily /st 08:00 /ru SYSTEM /rl highest /f
      ```

---

## 🔍 未解決的問題

- `sst.cs` 程式碼修正需要 rebuild SSTTray 才能完全生效（目前為 source code 層級修改）
- SSTTray 專案為 .NET Framework，不能用 `dotnet build` 編譯，需用 Visual Studio 或 MSBuild

---

## 📁 關鍵文件路徑

| 文件 | 路徑 | 狀態 |
|------|------|------|
| sst.cs（shioajiLogin 路徑修正） | `SSTTray/sst.cs` | ✅ 已修改 |
| 維護記錄 | `Docs/Maintenance/MAINTENANCE-LOG.md` | ✅ 已建立 |
| 交接檔 | `Docs/Maintenance/CURRENT_HANDOFF.md` | ✅ 已建立 |
| Shioaji 排程安裝批次檔 | `../install_shioaji_task.bat` | ✅ 已建立 |

---

## 📝 Session 歷史摘要

| Session | 日期 | 完成 | 階段 |
|---------|------|------|------|
| #1 | 2026-06-18 | 初始探索、Export API 設計 | 設計 |
| #2 | 2026-06-19 | 增量 Export API 實作 | 開發 |
| #3 | 2026-06-20 | detector/insertAlertList 重構、Shioaji VBS 啟動 | 開發 |
| **#4** | **2026-06-23** | **Shioaji Server 故障排除 & auto-start 修復、SST.StockImport.API 啟動修復** | **維運** |
