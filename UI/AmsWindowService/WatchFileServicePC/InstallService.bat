echo Installing service...
rem DO NOT remove the space after "binpath="!
sc create "WatchFileService" binpath="%~dp0WatchFileService.exe"
net start "WatchFileService"
pause