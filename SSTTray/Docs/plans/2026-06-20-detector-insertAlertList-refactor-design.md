# 設計文檔：detector() + insertAlertList() 重構

**日期**: 2026-06-20  
**系統**: SSTTray  
**工程師**: Scott Tseng  
**版本**: v1 (設計稿)

---

## 目標

對 `detector()` 與 `insertAlertList()` 進行中度重構（抽出共享邏輯），修復已知 bug，不改行為。

---

## 變更清單

### Fix 1: `insertAlertList()` pav direction=0 bug

**位置**: `sst.cs` L1820-1821

**現狀**:
```csharp
else if (pType == "pav" && direction == 0)
    onDupStr = "panVol5CntPos = panVol5CntPos+1";  // ❌ BUG: 下跌卻加 panVol5CntPos
```

**修正**:
```csharp
else if (pType == "pav" && direction == 0)
    onDupStr = "panVol5CntNeg = panVol5CntNeg+1";  // ✅ 改為加 panVol5CntNeg
```

---

### Fix 2: 移除 `detector()` 內的 `CommonClass.wait(1)`

**位置**: `TaskTrayApplicationContext.cs` `detector()` 方法

**現狀**: 7 個 try-catch SQL 區塊之間各有 `CommonClass.wait(1)`，浪費 7 秒。

**修正**: 全部移除。這些 SQL UPDATE 各自獨立，無需等待。

---

### Fix 3: `processAlert()` 記憶體追蹤取代重複 DB 查詢

**位置**: `sst.cs` `processAlert()` L2409-2430

**現狀**: 每檔股票每次 tick，在剛 `insertAlertList()` 更新完 alertlist 後，又從 DB `SELECT pDiff` 和 `SELECT panvolScore`。

**修正**:
- `pDiff`：在 `processAlert()` 區域變數追蹤，每次 `insertAlertList()` 根據 direction 增減
- `panvolScore`：在 `processAlert()` 區域變數累計，無需重查

---

### Fix 4: pType/direction → 欄位對應 Dictionary

**位置**: `sst.cs` `insertAlertList()` 方法

**現狀**: ~40 行 if-else if 鏈，pType/direction 到 SQL 欄位名的對應寫了兩次（一次 `onDupStr`，一次 INSERT VALUES）

**修正**: 建立靜態 `AlertTypeMap` Dictionary：

```csharp
private class AlertColumnMap
{
    public string Column { get; init; }      // SQL 欄位名
    public int InsertValue { get; init; }    // INSERT 時的初始值
}

private static readonly Dictionary<(string pType, int dir), AlertColumnMap> AlertTypeMap = new()
{
    [("pam", 1)]   = new() { Column = "paRatePosCnt",    InsertValue = 1 },
    [("pam", 0)]   = new() { Column = "paRateNegCnt",    InsertValue = 1 },
    [("plv", 1)]   = new() { Column = "pLVRatePosCnt",   InsertValue = 1 },
    [("plv", 0)]   = new() { Column = "pLVRateNegCnt",   InsertValue = 1 },
    [("p5v", 1)]   = new() { Column = "p5VRatePosCnt",   InsertValue = 1 },
    [("p5v", 0)]   = new() { Column = "p5VRateNegCnt",   InsertValue = 1 },
    [("pav", 1)]   = new() { Column = "panVol5CntPos",   InsertValue = 1 },
    [("pav", 0)]   = new() { Column = "panVol5CntNeg",   InsertValue = 1 },
    [("pa10v", 1)] = new() { Column = "pApRatePosCnt",   InsertValue = 1 },
    [("pa10v", 0)] = new() { Column = "pApRateNegCnt",   InsertValue = 1 },
    [("pa20v", 1)] = new() { Column = "panVol50CntPos",  InsertValue = 1 },
    [("pa20v", 0)] = new() { Column = "panVol50CntNeg",  InsertValue = 1 },
};
```

`onDupStr` 和 INSERT VALUES 從 map 自動生成。

---

### Fix 5: 統一「主進主出」判斷邏輯

**位置**: `sst.cs` `processAlert()` L2276-2344

**現狀**: 量級判斷（20x/10x/5x 主進主出）有獨立的 if-else 鏈，與 `calcPanVolCntsSimple()` 邏輯重疊。

**修正**: 建立 `calcPanLevel()` 方法取代 inline 判斷：

```csharp
// 回傳: 20/10/5/1/0/-1 表示不同的量級與方向
private static int calcPanLevel(double panAvgVolRate, int panVol, double panAmtRate, string stockType)
{
    if ((panAvgVolRate >= 20 || panVol >= 950) && panAmtRate > 0) return 20;
    if ((panAvgVolRate >= 20 || panVol >= 950) && panAmtRate < 0) return -20;
    if ((panAvgVolRate >= 10 || panVol >= 600) && panAmtRate > 0) return 10;
    if ((panAvgVolRate >= 10 || panVol >= 600) && panAmtRate < 0) return -10;
    int simple = calcPanVolCntsSimple(panWeight, panVol, panAvgVolRate, panAmtRate, stockType);
    if (simple == 1) return 5;
    if (simple == -1) return -5;
    if (simple == 0) return 0;
    return -99;
}
```

`processAlert()` 中改為 switch 決定 pType。

---

## 不改的事項

- `insertAlertList()` 的 method signature 不變
- `detector()` 寫入 DB 的值不變
- SQL 語句的內容（值、欄位名）不變，只改變生成方式
- 不引入新的 class library 或 NuGet 套件

---

## 驗證方式

1. 編譯通過
2. 與現有行為一致（產生的 SQL INSERT/UPDATE 值與重構前相同）
3. `lsp_diagnostics` 無錯誤
