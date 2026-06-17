#!/bin/bash
# ============================================================
# protect-reference-code.sh
# PreToolUse Hook: 在任何寫入操作前檢查是否為參考程式碼路徑
# 命中 → exit 2（Hard block，工具執行被停止）
# ============================================================

# 設定：請各專案修改此陣列為實際參考程式碼路徑
REFERENCE_PATHS=(
  "//*/share/legacy/*"
  "/mnt/legacy/*"
)

# 由 Claude Code Hook 框架傳入的檔案路徑
FILE_PATH="$1"

for pattern in "${REFERENCE_PATHS[@]}"; do
  if [[ "$FILE_PATH" == $pattern ]]; then
    echo ""
    echo "⛔ BLOCKED: Reference code is READ-ONLY"
    echo "   File: $FILE_PATH"
    echo "   This path is for reference only. Modification is not allowed."
    echo ""
    echo "   If this is a legitimate exception, you must:"
    echo "   1. Contact the user to confirm"
    echo "   2. Have user temporarily disable this hook"
    echo ""
    exit 2  # Hard block
  fi
done

exit 0  # Allow
