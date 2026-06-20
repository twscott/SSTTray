# detector() + insertAlertList() Refactor Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 中度重構 detector() 與 insertAlertList()，修 bug + 抽出共享邏輯，不改行為。

**Architecture:** 5 個獨立任務，可分批執行。Task 1-2 修正明確 bug，Task 3-4 重構結構，Task 5 清理 processAlert()。全部只改 sst.cs 與 TaskTrayApplicationContext.cs。

**Tech Stack:** .NET Framework 4.7.2 / C# / WinForms

---

### Task 1: 修 `insertAlertList()` pav direction=0 bug

**Files:**
- Modify: `SSTTray/sst.cs:1820-1821`

**Step 1:** 確認 bug 位置

在第 1820-1821 行，`pType == "pav"` 且 `direction == 0`（下跌）時，`onDupStr` 誤加 `panVol5CntPos` 應改為 `panVol5CntNeg`。

```csharp
// 原始（有 bug）：
else if (pType == "pav" && direction == 0)
    onDupStr = "panVol5CntPos = panVol5CntPos+1";

// 修正後：
else if (pType == "pav" && direction == 0)
    onDupStr = "panVol5CntNeg = panVol5CntNeg+1";
```

**Step 2:** 編譯驗證

Run: Build 方案確認無錯誤

**Step 3:** Commit

```bash
git add SSTTray/sst.cs
git commit -m "fix(insertAlertList): pav direction=0 should increment panVol5CntNeg not panVol5CntPos"
```

---

### Task 2: 移除 `detector()` 內的 `CommonClass.wait(1)`

**Files:**
- Modify: `SSTTray/TaskTrayApplicationContext.cs:621,631,641,668,684,703,719,731`

**Step 1:** 移除所有 `CommonClass.wait(1)` 呼叫

`detector()` 方法中有 7 個 try-catch SQL 區塊，每個區塊後都有 `CommonClass.wait(1)`。全部移除。

搜尋 `detector()` 方法內的所有 `CommonClass.wait(1)` 行，逐行刪除。

確認只刪 `wait(1)`，不刪 SQL 語句或其他邏輯。

**Step 2:** 編譯驗證

Run: Build 方案確認無錯誤

**Step 3:** Commit

```bash
git add SSTTray/TaskTrayApplicationContext.cs
git commit -m "perf(detector): remove unnecessary CommonClass.wait(1) between SQL blocks"
```

---

### Task 3: pType/direction → 欄位對應 Dictionary

**Files:**
- Modify: `SSTTray/sst.cs:1793-1866`

**Step 1:** 在 class `sst` 內新增 `AlertColumnMap` 類別與 `AlertTypeMap` Dictionary

在 `insertAlertList()` 方法之前加入：

```csharp
private class AlertColumnMap
{
    public string Column { get; init; }
    public int InsertValue { get; init; }
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
    [("pav", 0)]   = new() { Column = "panVol5CntNeg",   InsertValue = 1 },  // ← 已修 bug
    [("pa10v", 1)] = new() { Column = "pApRatePosCnt",   InsertValue = 1 },
    [("pa10v", 0)] = new() { Column = "pApRateNegCnt",   InsertValue = 1 },
    [("pa20v", 1)] = new() { Column = "panVol50CntPos",  InsertValue = 1 },
    [("pa20v", 0)] = new() { Column = "panVol50CntNeg",  InsertValue = 1 },
};
```

**Step 2:** 重構 `insertAlertList()` — 用 map 取代 if-else 鏈

```csharp
public static int insertAlertList(string stockID, string reason, string pType, int direction, 
    bool ifUpdateOnly = false, double panLastVolRate = 0, double panAvg5VolRate = 0, 
    double panAvgVolRate = 0, double panAmtRate = 0, int panVol = 0)
{
    string sqlStr = null;
    string onDupStr = null;
    try
    {
        var map = AlertTypeMap.GetValueOrDefault((pType, direction));
        if (map != null)
            onDupStr = $"{map.Column} = {map.Column}+1";

        onDupStr += $",maxPLVR=IF(maxPLVR > {panLastVolRate}, maxPLVR, {(direction == 0 ? -panLastVolRate : panLastVolRate)}), " +
            $" maxP5VR=IF(maxP5VR > {panAvg5VolRate}, maxP5VR, {(direction == 0 ? -panAvg5VolRate : panAvg5VolRate)}), " +
            $" maxPAR=IF(maxPAR > {panAvgVolRate}, maxPAR, {(direction == 0 ? -panAvgVolRate : panAvgVolRate)}), " +
            $" currPanAmtRate = {panAmtRate}, currPanVol={panVol} ";

        if (!ifUpdateOnly)
        {
            var (colStr, valStr) = BuildInsertColumns(pType, direction, panLastVolRate, panAvg5VolRate, panAvgVolRate, panAmtRate, panVol);
            sqlStr = $"insert ignore into `alertlist` ({colStr}) Values ({valStr})";
        }

        CommonClass.execSQLNonQuery(sqlStr, Constants.ConnString, true);
        return 0;
    }
    catch (Exception ex)
    {
        CommonClass.writeLog("sstTtry", "insertAlertList()", 5, ex.Message, ex, "scott.tseng@firstohm.com.tw");
        return 0;
    }
}
```

輔助方法 `BuildInsertColumns()` 從 `AlertTypeMap` 與固定欄位組合出 INSERT 的 columns/values 字串。

**Step 3:** 比較重構前後的 SQL 輸出

逐項確認 INSERT VALUES 的數字與重構前一致：
- `pam/1` → `paRatePosCnt = 1`
- `pam/0` → `paRateNegCnt = 1`
- `pav/0` → `panVol5CntNeg = 1`（已修 bug，行為改變）
- 其餘 pType/direction 組合不變

**Step 4:** 編譯驗證

Run: Build 方案確認無錯誤

**Step 5:** Commit

```bash
git add SSTTray/sst.cs
git commit -m "refactor(insertAlertList): replace if-else chain with AlertTypeMap dictionary"
```

---

### Task 4: 統一「主進主出」判斷邏輯

**Files:**
- Modify: `SSTTray/sst.cs:2276-2344`

**Step 1:** 建立 `calcPanLevel()` 方法

在 `calcPanVolCntsSimple()` 旁邊新增：

```csharp
/// <summary>
/// 計算盤量等級：正數=上漲方向，負數=下跌方向
/// 20=20倍主進, -20=20倍主出, 10=10倍主進, -10=10倍主出, 
/// 5=主進, -5=主出, 0=平盤, -99=無訊號
/// </summary>
private static int calcPanLevel(double panAvgVolRate, int panVol, double panAmtRate, string stockType, double panWeight)
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

**Step 2:** 將 `processAlert()` 中的 if-else 鏈（L2276-2344）改為 switch

```csharp
// 取代原本 L2276-2344 的 if-else 鏈
int panLevel = calcPanLevel(panAvgVolRate, panVol, panAmtRate, stockType, panWeight);
switch (panLevel)
{
    case 20:
        ifMess = 1; messRise = 1;
        notifyStrTitle.Append($"20 主進或盤量 >=950，瞬量拉升||");
        insertAlertList(msgArray.c, "20 主進", "pa20v", 1, false, ...);
        priority = 4;
        break;
    case -20:
        ifMess = 1; messFall = 1;
        notifyStrTitle.Append($"20 主出或盤量 >=950，瞬量下殺||");
        insertAlertList(msgArray.c, "20 主出", "pa20v", 0, false, ...);
        priority = 4;
        break;
    // ... 依此類推
}
```

**Step 3:** 編譯驗證

Run: Build 方案確認無錯誤

**Step 4:** Commit

```bash
git add SSTTray/sst.cs
git commit -m "refactor(processAlert): unify volume level detection with calcPanLevel()"
```

---

### Task 5: `processAlert()` 記憶體追蹤取代 DB 查詢

**Files:**
- Modify: `SSTTray/sst.cs:2031-2548`

**Step 1:** 在 `processAlert()` 的 `foreach` 之外新增記憶體累計變數

```csharp
// 在 foreach (MsgArray msgArray in priceResults) 之前
Dictionary<string, int> memPDiff = new Dictionary<string, int>();      // stockID → pDiff
Dictionary<string, int> memPanvolScore = new Dictionary<string, int>(); // stockID → panvolScore
```

**Step 2:** 每次 `insertAlertList()` 後更新記憶體值

```csharp
// 在 insertAlertList() 呼叫之後
if (!memPDiff.ContainsKey(msgArray.c))
    memPDiff[msgArray.c] = 0;
memPDiff[msgArray.c] += (direction == 1 ? 1 : direction == 0 ? -1 : 0);

if (!memPanvolScore.ContainsKey(msgArray.c))
    memPanvolScore[msgArray.c] = 0;
memPanvolScore[msgArray.c] += (int)((panAvgVolRate / 10) * (panAmtDiff > 0 ? 1 : -1));
```

**Step 3:** 用記憶體值取代原本的 SELECT 查詢

```csharp
// 取代原本 L2409-2416 的 SELECT pDiff 查詢
pDiff = memPDiff.GetValueOrDefault(msgArray.c, 0);

// 取代原本 L2419-2428 的 SELECT panvolScore 查詢
panvolScore = memPanvolScore.GetValueOrDefault(msgArray.c, 0);
if (panAmtDiff != 0 && panAvgVolRate >= 10)
    panvolScore += (int)((panAvgVolRate / 10) * (panAmtDiff > 0 ? 1 : -1));
```

**Step 4:** 編譯驗證

Run: Build 方案確認無錯誤

**Step 5:** Commit

```bash
git add SSTTray/sst.cs
git commit -m "perf(processAlert): replace per-tick DB queries with in-memory tracking"
```
