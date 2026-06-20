' Shioaji HTTP API server — 完全隱藏啟動（無 terminal 視窗）
' SSTTray 若偵測到 server 不在，也會用 Hidden 模式自動啟動此 script
' 因此這個檔案主要提供給開機捷徑使用

Dim WshShell
Set WshShell = CreateObject("WScript.Shell")

' 0 = 隱藏視窗，False = 不等待執行結束
WshShell.Run "cmd /c D:\ScottWork\SSTTray\start_shioaji.bat", 0, False
Set WshShell = Nothing
