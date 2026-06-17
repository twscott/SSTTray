# SSTTray Project Guidelines

**Before starting any task, read `AGENTS.md` for the complete maintenance strategy layer.**

This file contains system-specific supplements. The actual maintenance principles, iron rules, and workflow are defined in `AGENTS.md` (project root) and the company maintenance standards at `Docs/`.

---

## 維護規範快速索引

| 文件 | 位置 | 用途 |
|------|------|------|
| 維護策略層 | `AGENTS.md`（專案根目錄） | 4 鐵律、開發原則、文件地圖 |
| 系統基本資料 | `Docs/SYSTEM-PROFILE.md` | 技術棧、DB 資訊、保護檔案 |
| 維護憲法 | 公司規範倉庫 | 完整流程、10 Steps |
| 維護記錄 | `Docs/Maintenance/` | 本次維護任務追蹤 |

## Overview
.NET Framework 4.7.2 Windows Forms system tray application with MySQL backend, multi-language (Chinese/English), and extensive external integrations (LINE, FTP, Excel, UDP).

## Code Style

**Namespaces**: `FirstOhm` for business logic, `TaskTrayApplication` for UI  
**Naming**: Chinese mixed with English is acceptable throughout. Forms use `Form*` prefix or direct Chinese names (e.g., [Form產生Excel.cs](Form產生Excel.cs), [Form電鍍報表cs.cs](Form電鍍報表cs.cs))  
**Libraries**: Utility classes named `*Lib.cs` pattern ([LineLib.cs](LineLib.cs), [FTPLib.cs](FTPLib.cs), [ExcelLib.cs](ExcelLib.cs), [udpLib.cs](udpLib.cs))

## Architecture

**Entry Point**: [Program.cs](Program.cs) → `TaskTrayApplicationContext` (not a Form)  
**Main Controller**: [TaskTrayApplicationContext.cs](TaskTrayApplicationContext.cs) manages NotifyIcon, context menus, background timers, and UDP server  
**Core Utilities**: [CommonClass.cs](CommonClass.cs) (4000+ lines), [GlobalConst.cs](GlobalConst.cs), [appCommon.cs](appCommon.cs)  
**Data Pattern**: Heavy use of `DataTable` for data manipulation (prefer over custom objects)

## Database Access (MySQL)

**Read**: `CommonClass.getSQLDataTable(sqlStr, connString)` returns `DataTable`  
**Write**: `CommonClass.execSQLNonQuery(sqlStr, connString)`  
**Parameterized**: `CommonClass.execSQLNonQueryParams(sqlStr, paramDict, connString)`  
**Transactions**: `CommonClass.execSQLNonQueryTransactList(List<string>, connString)`  
**Connection**: Default is `Constants.ConnString` or `Constants.SSTConnString` from [GlobalConst.cs](GlobalConst.cs#L17-L54)

## Configuration

**File**: `C:\SSTTray\Property.txt` (key=value format, semicolon for comments)  
**Load**: `Constants.prepareProperty(path)` → `Dictionary<string, string>`  
**Access**: `Constants.getProperty("key", "defaultValue")`  
**Reload**: `Constants.reloadProperty()` for runtime updates  
See [GlobalConst.cs](GlobalConst.cs#L166-L197) for implementation.

## External Integrations

**LINE Messaging**: `LineLib.pushTextMessage(portalLoginName, msg)` using LineBotSDK  
**FTP**: `FTPExtensions` with `Upload(remotePath, localPath)` and `Download()` methods  
**Excel**: `ExcelLib` uses Microsoft.Office.Interop.Excel for DataTable↔Excel conversions  
**UDP Server**: JSON-serialized commands, runs on separate thread via `UDPLib.ThreadRunMethod()`

## Common Patterns

**Error Handling**: try-catch with `CommonClass.writeLog()` calls  
**Event Handlers**: Named `btn*_Click`, `*_Load` patterns  
**JSON**: Newtonsoft.Json: `JsonConvert.DeserializeObject<T>(jsonStr)`  
**Forms**: Init in constructor, singleton pattern for some (check [TaskTrayApplicationContext.cs](TaskTrayApplicationContext.cs))

## Build

```powershell
# Restore NuGet packages
nuget restore SSTTray.sln

# Build Debug
msbuild SSTTray.sln /p:Configuration=Debug

# Build Release
msbuild SSTTray.sln /p:Configuration=Release
```

**Output**: `bin\Debug\` or `bin\Release\`  
**Target Framework**: .NET Framework 4.7.2  
**Dependencies**: See [packages.config](packages.config)
