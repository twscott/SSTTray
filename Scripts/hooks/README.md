# Hook Scripts — Layer 2 Enforcement

## Available Hooks

| Hook | Type | Trigger | Behavior |
|------|------|---------|----------|
| `protect-files.ps1` | PreToolUse | Edit/Write | Blocks modification of protected files |
| `protect-prod-db.ps1` | PreToolUse | Bash | Blocks destructive SQL on production DB |
| `scan-hardcoded.ps1` | PostToolUse | Edit/Write | Scans for hardcoded secrets (warnings only) |

## Input Mode

All hooks support **dual-mode**:
- **stdin JSON** (OpenCode / universal): tool passes `{"tool_input":{"filePath":"..."}}` via pipe
- **Command-line arg** (Claude Code): `.\hook.ps1 "path"` / `.\hook.ps1 "$FILE"`

## Installation

### Claude Code — `.claude/settings.json`
```json
{ "matcher": "Edit|Write", "command": "powershell -NoProfile -File scripts/hooks/protect-files.ps1" }
```

### OpenCode — `opencode.json` (project root)
Same format, copy to `opencode.json`.

### Git Hook (Universal)
```powershell
.\scripts\hooks\install-git-hooks.ps1
```

## Exit Codes

| Code | AI Tool Hook | Git Hook |
|------|-------------|----------|
| 0 | Allow | Pass |
| 1 | — | Block |
| 2 | Block | — |
