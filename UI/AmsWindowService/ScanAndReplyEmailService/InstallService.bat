echo Installing service...
rem DO NOT remove the space after "binpath="!
sc create "Ams Scan And Reply Email Service" binpath="%~dp0ScanAndReplyEmailService.exe"
net start "Ams Scan And Reply Email Service"
pause