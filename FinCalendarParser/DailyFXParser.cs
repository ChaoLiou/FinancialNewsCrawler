﻿using CsQuery;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FinCalendarParser
{
    public class DailyFXParser
    {
        private static Dictionary<Locale, string> _APIUrlFTLocaleMap = new Dictionary<Locale, string>()
        {
            {Locale.ZhCN, "http://www.dailyfx.com.hk/calendar/getData/{0}-{1}-{2}/{3}-{4}-{5}"},
            {Locale.ZhTW, "http://www.dailyfx.com.hk/cn/calendar/getData/{0}-{1}-{2}/{3}-{4}-{5}"},
            {Locale.EnUS, "http://www.dailyfx.com/economic-calendar/selectedDateData/{0}-{1}-{2}"}
        };

        public Locale Locale { get; set; }
        public DailyFXParser(Locale locale)
        {
            Locale = locale;
        }

        public List<DailyFXEvent> Process(DateTime dateTimeFirstDay, PeriodType periodType)
        {
            var dateTimeLastDay = dateTimeFirstDay.AddXDays(periodType);
            if (Locale == Locale.EnUS)
            {
                var list = new List<DailyFXEvent>();
                var dateTimeTmp = DateTime.Parse(dateTimeFirstDay.ToString());

                while (dateTimeTmp.CompareTo(dateTimeLastDay) < 0)
                {
                    var dateTimeMonday = dateTimeTmp.StartOfWeek(DayOfWeek.Sunday);
                    var url = string.Format(_APIUrlFTLocaleMap[Locale],
                        dateTimeMonday.Year,
                        dateTimeMonday.Month.PaddingZero(),
                        dateTimeMonday.Day.PaddingZero());

                    list.AddRange(ProcessFromEnUS(url));
                    dateTimeTmp = dateTimeTmp.AddXDays(PeriodType.Week);
                }

                return list.Where(x => DateTime.Parse(x.Date.ToString("yyyy-MM-dd")).CompareTo(dateTimeFirstDay) >= 0 && DateTime.Parse(x.Date.ToString("yyyy-MM-dd")).CompareTo(dateTimeLastDay) < 0).ToList();
            }
            else
            {
                var url = string.Format(_APIUrlFTLocaleMap[Locale],
                    dateTimeFirstDay.Year,
                    dateTimeFirstDay.Month.PaddingZero(),
                    dateTimeFirstDay.Day.PaddingZero(),
                    dateTimeLastDay.Year,
                    dateTimeLastDay.Month.PaddingZero(),
                    dateTimeLastDay.Day.PaddingZero());
                return ProcessInZhCNnZhTW(url);
            }
        }

        private List<DailyFXEvent> ProcessFromEnUS(string url)
        {
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(url);
                var dataString = Encoding.UTF8.GetString(data);
                var raws = JsonConvert.DeserializeObject<List<DailyFXRaw_en>>(dataString);
                return raws.Select(x => 
                {
                    var dt = (new DateTime(1970, 1, 1)).AddSeconds(x.eventDate);
                    var timeString = dt.ToString(@"HH:mm");
                    return new DailyFXEvent(Locale.EnUS, dt.ToString(@"yyyy-MM-dd"))
                    {
                        Currency = x.currency.ToUpper(),
                        Description = x.title,
                        Importance = x.importance,
                        Forecast = x.forecast,
                        Actual = x.actual,
                        Previous = x.previous,
                        Memo = x.comment,
                        Time = timeString == "00:00" ? "" : timeString
                    };
                }).ToList();
            }
        }

        private List<DailyFXEvent> ProcessInZhCNnZhTW(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var dom = CQ.CreateFromUrl(url);
            return ProcessInZhCNnZhTW(dom);
        }

        private List<DailyFXEvent> ProcessInZhCNnZhTW(CQ dom)
        {
            var eventGroupsDaily = dom["tbody"];
            var list = new List<DailyFXEvent>();

            foreach (var eventGroup in eventGroupsDaily.Select(x => x.Cq()))
            {
                var datetimeInformation = eventGroup.Find("tr:first-child").Text().Trim();
                var events = eventGroup.Find("tr.dfx-calendar-item, tr.dfx-event-detail");
                DailyFXEvent re = null;

                foreach (var e in events.Select(x => x.Cq()))
                {
                    var fields = e.Find("td");
                    foreach (var fieldWithIndex in fields.Select((x, i) => new { Value = x.Cq(), Index = i }))
                    {
                        if (fieldWithIndex.Index < 9)
                        {
                            var propertyName = Config.ColumnMap[Locale][fieldWithIndex.Index];
                            if (!string.IsNullOrWhiteSpace(propertyName))
                            {
                                var text = fieldWithIndex.Value.Text().Trim();
                                if (fieldWithIndex.Index == 0)
                                {
                                    var m = Regex.Match(text, @"^\d{2}:\d{2}$");
                                    if (m.Success || string.IsNullOrWhiteSpace(text))
                                    {
                                        re = new DailyFXEvent(Locale, datetimeInformation);
                                    }
                                    else
                                    {
                                        re.Memo = text;
                                        continue;
                                    }
                                }

                                if (fieldWithIndex.Index < 8)
                                {
                                    typeof(DailyFXEvent).GetProperty(propertyName).SetValue(re, text);
                                }
                                else
                                {
                                    list.Add(re);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }
    }
}
