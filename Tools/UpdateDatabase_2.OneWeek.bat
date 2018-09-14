@echo off
set /p date="Enter Date(yyyy-mm-dd): "
set /p locale="Enter Locale(zh-tw/zh-cn/en-us): "
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=%locale% --period=week --doupdate
start chrome http://probis.infostrum.com.tw/MKIS/jspx/information?date=%date%^&locale=%locale%^&mode=^&week=true http://probis.infostrum.com.tw/MKIS/jspx/information?date=%date%^&locale=%locale%^&mode=backend^&week=true
pause