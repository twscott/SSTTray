# 遠端機器共同開發設定 — REMOTE-DEV-SETUP

> 目的：讓另一台機器 clone 本 repo 後能編譯、跑行情（B1）、佈建機密，與本機共同開發。
> 更新：2026-09-19。

## 1) 下載與分支

```powershell
git clone https://github.com/twscott/SSTTray.git
cd SSTTray
git checkout current        # 現行工作碼分支（含行情優化 + B1）
```

> 注意：GitHub 的 `master` 為舊佈局舊碼；**共同開發一律用 `current` 分支**（檔案佈局 = 本機現行工作碼）。

## 2) 建置環境（repo 不含這些）

| 需要 | 安裝 |
|---|---|
| Git | https://git-scm.com |
| Visual Studio 2022（或 BuildTools） | 勾選「.NET desktop」＋「.NET Framework 4.7.2 targeting pack」 |
| NuGet packages | VS 開啟 `SSTTray.sln` 自動還原；或 `nuget restore SSTTray.sln`（packages/ 已被 gitignore） |

編譯：`MSBuild SSTTray.sln /p:Configuration=Debug` → 產物 `bin\Debug\SSTTray.exe`。

## 3) 機密（一律環境變數／外部檔，不入 repo）

以 `scripts/set-secrets.ps1` 對帳＋佈建（值由工程師提供，**不要貼進對話或 repo**）：

```powershell
powershell -File scripts\set-secrets.ps1 -Check -FailOnMissing     # 檢查缺漏
```

必備變數（User 範圍）：`SST_DB_PWD`、`SST_DB_DIGGO_PWD`、`SST_LINE_APP_KEY`、`SST_LINE_KEY`、`SST_BAK_KEY_A~E`、`SST_SMTP_PWD`、`SST_FTP_USER`、`SST_FTP_PWD`、`SHIOAJI_API_KEY`、`SHIOAJI_SECRET_KEY` — 名稱與用途見 `Docs/SECRETS.md`（該檔在 AI workspace 專案，未入 repo；可向工程師索取副本）。

外部檔：
- `C:\SSTTray\Property.txt`（系統設定，保留不動）
- `C:\SSTTray\.env`（行情 B1 用，見下）

## 4) 行情（B1：官方 shioaji server）

```powershell
uv tool install shioaji                    # 或官方 install.ps1
# C:\SSTTray\.env（下方兩行 + 註解；正式環境再加 SJ_CA_* 並設 SJ_PRODUCTION=true）
# SJ_API_KEY=<值>
# SJ_SEC_KEY=<值>
```

程式會於交易日 **08:30 自動 `shioaji server start`、14:00 stop**；手動驗證：

```powershell
shioaji server check                       # healthy
Invoke-RestMethod http://127.0.0.1:8080/api/v1/info   # simulation=true 為模擬模式
```

> 模擬模式預設；**正式環境需先完成 CA 啟用**（另議，Gate 3）。

## 5) 資料庫（本機 MySQL）

- `sst`（production，**僅 SELECT 唯讀**）／`sstv2`（sandbox，測試/驗證用）
- 遠端機需有相同 DB 結構（可 `mysqldump` 自本機匯出後匯入 sstv2）；連線由 code 內 `GlobalConst.cs` Constants（密碼走 env `SST_DB_PWD`）提供

## 6) 每日共同開發流程

```powershell
git pull --rebase origin current      # 開工先同步
# 改碼 → 測試（跑到 sstv2 驗證）→
git add -A; git commit -m "說明"; git push origin current     # 或開 PR
```

- 衝突：解完 `<<<<< ===== >>>>>` 後 `git add` + `git rebase --continue`
- 建議一律以 PR 合併，避免直接推 `current`（可議）

## 7) 驗收清單（遠端機就緒判定）

- [ ] `shioaji server check` healthy（或 08:30 自動啟動後 `/api/v1/info` 200）
- [ ] `msbuild SSTTray.sln` 編譯通過
- [ ] `set-secrets.ps1 -Check -FailOnMissing` 全綠
- [ ] sstv2 連線測試（`mysql -u root sstv2 -e "select 1"`）
- [ ] 假日/交易日判斷正確（週一~五 09:00–13:30 主排程才動）