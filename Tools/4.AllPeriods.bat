@echo off
set /p date="Enter Date(yyyy-mm-dd): "
echo zh-tw
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=zh-tw --period=day
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=zh-tw --period=week
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=zh-tw --period=month
echo zh-cn
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=zh-cn --period=day
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=zh-cn --period=week
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=zh-cn --period=month
echo en-us
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=en-us --period=day
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=en-us --period=week
\\iserver01\CS\FinCalendarCrawler\FinCalendarCrawler.exe --startswith=%date% --locale=en-us --period=month
pause