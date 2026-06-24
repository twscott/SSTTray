# CURRENT_HANDOFF — SSTTray / 自動啟動修復

**Session 編號**：#5  
**收工時間**：2026-06-24 10:30  
**收工工程師**：Scott Tseng

---

## 📍 當前位置

| 欄位 | 內容 |
|------|------|
| **系統** | SSTTray（系統狀態列工具） |
| **系統代碼** | `ssttray` |
| **本次維護功能** | SSTTray + Shioaji Server 自動啟動（雙層架構） |
| **Git 分支** | master |
| **當前步驟** | 已完成，等待下次重開機驗證 |
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

### 問題
電腦每天 06:00 重開機，~10:00 才有人登入。Shioaji Server 與 SSTTray.exe 都不會自動啟動，導致 6:00~10:00 之間 4 小時空白。

### 根因
1. Task Scheduler「SSTTray Monitor」設 `InteractiveToken`，須使用者登入才能執行
2. 啟動資料夾有 `ShioajiServer.lnk` 但無 SSTTray 捷徑
3. 無 Windows 自動登入設定，重開機後無人登入就無法啟動

### 解決方案：雙層啟動架構

**系統層（06:00 重開機後不須登入）：**
- Windows 自動登入（Auto-Logon）設定：`FIRSTOHM\scott.tseng` 自動登入
- 啟動資料夾 `SSTTray.lnk` → `start_ssttray.bat`

**使用者層（登入後備援）：**
- Task Scheduler「SSTTray Monitor」每分鐘檢查，不在就重啟
- 備份原 `ShioajiServer.lnk` → `ShioajiServer.lnk.bak`

### `start_ssttray.bat` 統一起動腳本
- **Phase1**: Shioaji Server 啟動（`shioaji server start --no-open`）+ 健康檢查（最長等 60 秒）
- **Phase2**: SSTTray.exe 啟動（from `bin\Release\SSTTray.exe`）
- **Phase3**: 背景監控迴圈（每 60 秒檢查，掛了重啟）
- 完全等冪設計（已執行中 → 跳過，安全重複執行）
- 完整日誌寫入 `start_ssttray.log`

---

## ⏭️ 下次開工第一件事

**操作**：驗證下次重開機後自動啟動是否正常

**檢查項目**：
1. [ ] 06:00 重開機後，Windows 是否自動登入
2. [ ] SSTTray.exe 是否在 system tray 中（不要只看 taskbar）
3. [ ] Shioaji Server 是否正常（`curl http://127.0.0.1:8080/api/v1/health`）
4. [ ] `D:\ScottWork\SSTTray\start_ssttray.log` 是否有啟動記錄
5. [ ] 收盤後 alertlog 當日有資料（`mysql -u root sst -e "SELECT COUNT(*) FROM alertlog WHERE alertdate >= CURDATE()"`）

---

## ⚠️ 等待工程師處理的事項

- 無（本次為完整交付）

---

## 🔍 未解決的問題或 Uncertainties

- 無

---

## 📁 本次維護的關鍵檔案路徑

| 檔案 | 路徑 | 狀態 |
|------|------|------|
| 統一起動腳本 | `start_ssttray.bat` | ✅ 新建 |
| 啟動日誌（自動產生） | `start_ssttray.log` | ✅ 自動產生 |
| 設計文件 | `Docs/plans/2026-06-24-auto-startup-design.md` | ✅ 新建 |
| 實作計畫 | `Docs/plans/2026-06-24-auto-startup-plan.md` | ✅ 新建 |
| 啟動捷徑 | `%APPDATA%\...\Startup\SSTTray.lnk` | ✅ 新建 |
| Shioaji 原捷徑 | `%APPDATA%\...\Startup\ShioajiServer.lnk.bak` | ✅ 更名備份 |
| Task Scheduler | `\SSTTray Monitor`（動作 → start_ssttray.bat） | ✅ 更新 |
| Windows 登錄檔 | `HKLM\...\Winlogon\AutoAdminLogon` | ✅ 設定自動登入 |

---

## 📝 Session 歷史摘要

| Session | 日期 | 完成 | 階段 |
|---------|------|------|------|
| #1 | 2026-06-17 | First Apply + Shioaji Token 外移 | 首次接入 ✅ |
| #2 | 2026-06-19 | Shioaji HTTP API 遷移 + 重構 + 通知 | 維護完成 ✅ |
| #3 | 2026-06-20 | detector + insertAlertList 重構 + Shioaji 隱藏啟動 | 維護完成 ✅ |
| #4 | 2026-06-22 | 系統停擺診斷 + 重建 exe + Shioaji Server 啟動 | 待觀察 ✅ |
| **#5** | **2026-06-24** | **自動啟動修復（雙層架構 + Auto-Logon）** | **待驗證 ✅** |
