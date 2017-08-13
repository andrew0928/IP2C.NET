cd /d c:\nginx

md logs
md temp

:loop
ipconfig /flushdns
nginx.exe
powershell /c sleep 1
goto loop