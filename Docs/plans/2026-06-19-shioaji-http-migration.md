# Shioaji HTTP API 遷移計畫

> **目標**: 將 SSTTray 從舊的 Sinopac.Shioaji C# SDK (v1.1.8) 遷移至 Shioaji HTTP API

**原因**: 舊 SDK 的 API Key 驗證機制已過期，且 Shioaji 官方已轉以 Python CLI + HTTP API 為主要方案

**Architecture**:
```
shioaji server (daemon, port 8080)
  ↑ .env (API Key + CA憑證)
  ↓ HTTP POST /api/v1/data/snapshots
C# HttpClient → stockDictToMsgArray() → processAlert() → insertAlertList()
```

**不變的部分**: `do_sst()` 排程、`detector()` 彙總、`insertAlertList()` 寫入、Timer 邏輯

---

### Task 1: 程式碼遷移 — shioajiLogin() / shioajiStockAlert() / processContract()

**修改**: `SSTTray/sst.cs:929-969` (shioajiLogin), `:1060-1134` (shioajiStockAlert), `:1162-1217` (processContract)

**Step 1: 改 `shioajiLogin()`**
- 移除 `Sinopac.Shioaji` API 呼叫
- 改為檢查 `localhost:8080` 是否活著（GET /api/v1/health）
- 如果不在就 return false（由外部手動啟動 shioaji server）
- 移除 `public static Shioaji _api` 全域變數

**Step 2: 改 `shioajiStockAlert()`**
- 移除 `_api.Contracts.Stocks[...]` 取得合約的方式
- 改為直接用股票代碼組 JSON body
- 用 `HttpClient` POST 到 `http://127.0.0.1:8080/api/v1/data/snapshots`
- 每次請求最多 500 檔股票

**Step 3: 改 `stockDictToMsgArray()`**
- case `"ts"` → 改為 case `"datetime"`（HTTP API 回 ISO 字串）
- 解析 `DateTime.Parse()` 取代 Unix 奈秒換算

**Step 4: 改 `processContract()`**
- 移除舊 SDK 的 `_api.Snapshots(contracts)` 和 JavaScriptSerializer
- 直接接收 HttpClient response JSON → `List<Dictionary<string, object>>`
- 其他邏輯不變

### Task 2: 新增可疑資料 Logging

**修改**: `SSTTray/sst.cs` processAlert() 區塊 + `SSTTray/TaskTrayApplicationContext.cs`

**檢查條件**（滿足任一就寫 log）：
1. `onTimeVol < preVol` — 累計量倒退（不該發生）
2. `panVol < 0` — 計算出的本盤量為負
3. `panAvgVolRate > 100` — 倍數極端異常（選擇性）

**寫入方式**: 呼叫 `CommonClass.writeLog("sstTray", "suspiciousData", 3, 訊息)` 
log 內容包含：股票代號、時間、onTimeVol、preVol、panVol、panAvgVolRate、懷疑原因

### Task 3: 移除舊 Shioaji NuGet 參考

**修改**: `SSTTray\SSTTray.csproj` — 移除 Sinopac.Shioaji Reference
**清理**: 移除 `sst.cs` 中 `using Sinopac;` 和 `using Sinopac.Shioaji;`

### Task 4: 建立部署文件

**新增**: `docs/plans/shioaji-server-setup.md`
- .env 設定說明
- 開機自動啟動設定方式
- 檢查 server 狀態的指令

---

## 完成定義

- [ ] `shioajiLogin()` 改為 HTTP health check
- [ ] `stockDictToMsgArray()` 可正確解析 HTTP API 的 ISO datetime
- [ ] `shioajiStockAlert()` 用 HttpClient 成功取得 snapshot
- [ ] 可疑 volume 資料會寫入 log
- [ ] 舊 Shioaji NuGet 套件已移除
- [ ] 專案編譯成功無錯誤
- [ ] 部署文件已建立
