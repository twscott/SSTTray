# CURRENT_HANDOFF — SSTTray / Shioaji HTTP API 遷移 + 重構

**Session 編號**：#2  
**收工時間**：2026-06-19 10:45  
**收工工程師**：Scott Tseng

---

## 📍 當前位置

| 欄位 | 內容 |
|------|------|
| **系統** | SSTTray（系統狀態列工具） |
| **系統代碼** | `ssttray` |
| **本次維護功能** | Shioaji API 認證修復（HTTP API 遷移） + 程式碼重構 + 通知機制 |
| **Git 分支** | master |
| **當前步驟** | 已完成（等待下次維護需求） |
| **總體進度** | 100% ✅ |

---

## 🚥 Gate 狀態

| Gate | 狀態 | 日期 | 備註 |
|------|------|------|------|
| **Gate 1**（UC 審核） | 🔲 未到 | — | 本次非完整維護週期 |
| **Gate 2**（GM 驗證） | 🔲 未到 | — | 本次非完整維護週期 |
| **Gate 3**（部署授權） | 🔲 未到 | — | 本次非完整維護週期 |

---

## ✅ 本次 Session 完成的事項

1. Shioaji 認證修復：發現 root cause 為 Shioaji API Key 過期 + CA 憑證未啟用
2. 重新申請 Shioaji API Key，取得新憑證並驗證可用
3. 舊 `Sinopac.Shioaji` NuGet SDK 完全移除，改為 Shioaji HTTP API
4. `shioajiStockAlert()` 重寫為 HttpClient POST 方式
5. `stockDictToMsgArray()` datetime 格式相容
6. `shioajiLogin()` 改為 health check + 自動啟動 server + CA 到期檢查
7. `detector()` 錯誤處理重構（5 組 try-catch 抽成共用 helper）
8. `processAlert()` 重複驗證抽取（skipStock helper）+ 清理未用變數
9. 新增可疑 volume 資料 logging
10. 新增每日 13:35 資料健康檢查（0 筆寄信）
11. 新增 CA 憑證到期前 30 天 Line 通知
12. 新增 Shioaji server 開機自動啟動（Windows Startup）
13. `sst.cs:939` 環境變數讀取保持不變

---

## ⏭️ 下次開工第一件事

**操作**：等待工程師提出新的維護需求

**原因**：本次工作已全部完成，無進行中的維護任務

**注意**：下個交易日須觀察 alertlist 資料是否正常恢復收集

---

## ⚠️ 等待工程師處理的事項

- 無

---

## 🔍 未解決的問題或 Uncertainties

- FirstohmService 監視的路徑是否要改成 Debug 版（已確定維持 Debug）
- 如日後改用 Release 版，`bin\Release\SSTTray.exe` 已編譯好可用

---

## 📁 本次維護的關鍵檔案路徑

| 檔案 | 路徑 | 狀態 |
|------|------|------|
| 核心遷移 | `SSTTray/sst.cs` | ✅ 已修改（HTTP API + logging） |
| Timer 重構 | `SSTTray/TaskTrayApplicationContext.cs` | ✅ 已修改（detector + 每日檢查） |
| 專案檔 | `SSTTray/SSTTray.csproj` | ✅ 已修改（移除舊 NuGet） |
| 實作計畫 | `docs/plans/2026-06-19-shioaji-http-migration.md` | ✅ 新建 |
| 部署腳本 | `start_shioaji.bat` | ✅ 新建 |
| .gitignore | `.gitignore` | ✅ 更新 |
| 環境設定 | `D:\ScottWork\SSTTray\.env` | ✅ 新建（含 API Key，不進 git） |
| 開機捷徑 | `C:\Users\Scott.Tseng\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\ShioajiServer.lnk` | ✅ 已建立 |

---

## 📝 Session 歷史摘要

| Session | 日期 | 完成 | 階段 |
|---------|------|------|------|
| #1 | 2026-06-17 | First Apply + Shioaji Token 外移 | 首次接入 ✅ |
| #2 | 2026-06-19 | Shioaji HTTP API 遷移 + 重構 + 通知 | 維護完成 ✅ |
