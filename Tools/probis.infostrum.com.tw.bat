@echo off
set /p date="Enter Date(yyyy-mm-dd): "
set /p locale="Enter Locale(zh-tw/zh-cn/en-us): "
start chrome http://probis.infostrum.com.tw/MKIS/jspx/information?date=%date%^&locale=%locale%^&mode=  http://probis.infostrum.com.tw/MKIS/jspx/information?date=%date%^&locale=%locale%^&mode=backend
pause