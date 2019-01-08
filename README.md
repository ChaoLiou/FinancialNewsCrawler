# FinancialNewsCrawler

- This is a console application for crawling `dailyfx's Economic Calendar`, including [zh-tw](https://www.dailyfx.com.hk/cn/calendar), [zh-cn](https://www.dailyfx.com.hk/calendar) and [en-us](https://www.dailyfx.com/calendar) sites, and it will export `*.csv` files or update through probis financial calendar api to get the latest data.
- There are different ways to use:
    - executing with no arguments -> it will show all required or optional arguments you can pass in (case-insensitive).
        - > `Helper: FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw|zh-cn|en-us [--Period=day|week|month] [--DoUpdate]`
        - required: --StartsWith, --Locale
        - optional: --Period(default is `week`), --DoUpdate
​
|||
|---|---|
|crawling on `zh-tw` site in one day|`FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw --Period=day`|
|crawling on `zh-tw` site in execute one week|`FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw --Period=week`|
|crawling on `zh-tw` site in execute one month|`FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw --Period=month`|
|crawling on `zh-tw` site in one day **and update**|`FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw --Period=day --DoUpdate`|
|crawling on `zh-tw` site in one week **and update**|`FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw --Period=week --DoUpdate`|
|crawling on `zh-tw` site in one month **and update**|`FinCalendarCrawler.exe --StartsWith=2018-01-01 --Locale=zh-tw --Period=month --DoUpdate`|
