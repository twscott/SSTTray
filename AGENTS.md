# AGENTS.md — SSTTray Strategy Layer

**Strategic North Star**: 讓工程師睡得著的系統維護

---

## ⛔ 維護 4 鐵律

1. **生產環境神聖不可侵犯** — Production DB（sst）僅 SELECT 唯讀
2. **無 Golden Master 不得開發** — 改 code 前錄製 GM 基準
3. **新舊功能必須共存** — 保留舊入口至少 2 週
4. **工程師 Gate 簽核** — 三個 Gate 須明確確認

---

## 🧠 開發原則

1. **Think Before Coding** — 說出假設，不確定就問
2. **Simplicity First** — 最小變更，不多做
3. **Surgical Changes** — 只動該動的，V2 命名不改舊碼
4. **Goal-Driven Execution** — GM 驗證通過 = 完成

---

## 🗺️ 文件地圖

詳細規範在 `Docs/` 按需查閱。

| 查閱目的 | 文件 |
|----------|------|
| 系統基本資料 | `Docs/SYSTEM-PROFILE.md` |
| 功能探索報告 | `Docs/Discovery/` |
| 維護 UC 文件 | `Docs/UC/` |
| 維護記錄 | `Docs/Maintenance/` |

---

## 系統專屬規則

- 🔒 **保護檔案（不可修改）**：`SSTTray/GlobalConst.cs`
- 📁 **外部設定檔（保留不動）**：`C:\SSTTray\Property.txt`
- 🗄️ **Production DB**：`sst`（本機 127.0.0.1）
- 🧪 **Sandbox DB**：`sstv2`（本機 127.0.0.1）
- 🏗️ 本系統為 System Tray App（非 MDI 表單），共存的雙入口實作方式須與工程師討論
