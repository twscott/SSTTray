# 維護記錄 — SSTTray

> **建立日期**: 2026-06-17

---

## 維護歷史

### 2026-06-17 — 系統首次接入（First Apply）+ Shioaji Token 外移

**完成項目**：
- 從 companyMaintains 倉庫取得最新維護規範並套用
- 建立 SYSTEM-PROFILE.md（系統基本資料）
- 建立 AGENTS.md（核心策略層）
- 安裝 scripts/hooks/ 保護腳本 + Git Hooks
- 更新 .gitignore 保護 SYSTEM-PROFILE.md
- 更新 copilot-instructions.md
- 將 Shioaji API Key/Secret 從 sst.cs 硬編碼移至 Windows 使用者環境變數

**產出文件**：
- `Docs/SYSTEM-PROFILE.md`（新建）
- `AGENTS.md`（新建）
- `scripts/hooks/`（新建）
- `Docs/Maintenance/MAINTENANCE-LOG.md`（新建）
- `SSTTray/.github/copilot-instructions.md`（更新）
- `.gitignore`（更新）
- `SSTTray/sst.cs`（修改 - line 939 改為讀取環境變數）

**Gate 狀態**：未觸達（本次為首次接入 + 小修改，非完整維護週期）

**下次繼續**：無待辦事項。下次開工時由工程師提出新維護需求。
