@echo off
set /p date="Enter Date(yyyy-mm-dd): "
set /p locale="Enter Locale(zh-tw/zh-cn/en-us): "
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=%locale% --period=month
pause