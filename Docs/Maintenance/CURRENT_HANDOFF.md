# CURRENT_HANDOFF — SSTTray / 資料修復 + MACD 本地計算

**Session 編號**：#6  
**收工時間**：2026-06-27  
**收工工程師**：Scott Tseng

---

## 📍 當前位置

| 欄位 | 內容 |
|------|------|
| **系統** | SSTTray（系統狀態列工具） |
| **系統代碼** | `ssttray` |
| **本次維護功能** | 欄位錯位修復 + weekall 同步重建 + MACD 本地計算 |
| **Git 分支** | master |
| **當前步驟** | 已完成 |
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

### sst.cs 新增函數

| 函數 | 說明 |
|------|------|
| `syncWeekallClosingPrice()` | 取代舊版 SDK 的 `syncClosingPrice`，使用 HTTP API 批次同步 weekall |
| `calcMACD()` | 從 tradedata 收盤價計算 DIF/MACD/OSC（不依賴 Goodinfo） |

### 修復的 Bug

| Bug | 位置 | 說明 |
|-----|------|------|
| stock60days MV14/MV20 寫錯 | `sst.cs:1582` | `MV14`={mv14},`MV14`={mv20} → MV20 永不寫入 |
| weekall 興櫃 OpenPriec=0 | 同步函數 | Open=0 時 fallback 到 Close |
| syncTradedata/MACD 缺失 | 已重建 | 對應函數重寫 |
| Goodinfo MACD 匯入失效 | `calcMACD()` | 改用本地計算取代外部來源 |

### 資料修復

| 資料表 | 異常 | 修復前 | 修復後 |
|--------|------|--------|--------|
| weekall 興櫃 | OpenPriec=0 | 27,167 | **754**（無交易日） |
| weekall 興櫃 | Open<Low | 26,621 | **0** |
| tradedata | StockID=StockName | 3,303 | **0** |

### 自動啟動機制（Session #5 延續）

- Windows Auto-Logon ✅
- 啟動資料夾 `SSTTray.lnk` → `start_ssttray.bat` ✅
- Task Scheduler「SSTTray Monitor」✅
- 6/25 早上已成功自動啟動（log 驗證）✅

---

## ⏭️ 下次開工第一件事

1. 執行 `sst.calcMACD()` 補回所有 MACD/DIF/OSC 資料（首次較久，之後只跑增量）
2. 如有需要執行 `sst.calcStock60Days()` 重新計算 MA/MV（會自動覆蓋舊資料）
3. 觀察下次交易日 alertlog 資料是否正常收集

---

## ⚠️ 等待工程師處理的事項

- 無

---

## 🔍 未解決的問題或 Uncertainties

- Goodinfo MACD 下載失效原因不明（3月起停），已用本地 `calcMACD()` 替代
- MV14/MV20 現有資料因 bug 錯位，下次跑 `calcStock60DaysByStockID()` 會自動重新計算

---

## 📁 本次維護的關鍵檔案路徑

| 檔案 | 路徑 | 狀態 |
|------|------|------|
| 股票分析核心 | `SSTTray/sst.cs` | ✅ 修改（新增 2 函數 + 修正 1 bug） |
| 交接文件 | `Docs/Maintenance/CURRENT_HANDOFF.md` | ✅ 更新 |

---

## 📝 Session 歷史摘要

| Session | 日期 | 完成 | 階段 |
|---------|------|------|------|
| #1 | 2026-06-17 | First Apply + Shioaji Token 外移 | ✅ |
| #2 | 2026-06-19 | Shioaji HTTP API 遷移 + 重構 + 通知 | ✅ |
| #3 | 2026-06-20 | detector + insertAlertList 重構 | ✅ |
| #4 | 2026-06-22 | 系統停擺診斷 + 重建 exe | ✅ |
| #5 | 2026-06-24 | 自動啟動修復（雙層架構 + Auto-Logon） | ✅ |
| **#6** | **2026-06-27** | **資料修復 + MACD 本地計算** | **✅ 完成** |
