using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace FinCalendarParser
{
    public class Parser
    {
        public Locale Locale { get; set; }
        public Parser(Locale locale)
        {
            Locale = locale;
        }

        public List<RawEvent> Process(DateTime dateTime_first, PeriodType periodType)
        {
            var dateTime_last = dateTime_first.AddXDays(periodType);
            if (Locale == Locale.EnUS)
            {
                var list = new List<RawEvent>();
                var dateTime_tmp = DateTime.Parse(dateTime_first.ToString());

                while (dateTime_tmp.CompareTo(dateTime_last) < 0)
                {
                    var dateTime_monday = dateTime_tmp.StartOfWeek(DayOfWeek.Sunday);
                    var url = string.Format(Settings.WebAPIUrlFormatMap[Locale],
                        dateTime_monday.Year,
                        dateTime_monday.Month.PaddingZero(),
                        dateTime_monday.Day.PaddingZero());

                    list.AddRange(processFrom_enUS(url));
                    dateTime_tmp = dateTime_tmp.AddXDays(PeriodType.Week);
                }

                return list.Where(x => DateTime.Parse(x.Date.ToString("yyyy-MM-dd")).CompareTo(dateTime_first) >= 0 && DateTime.Parse(x.Date.ToString("yyyy-MM-dd")).CompareTo(dateTime_last) < 0).ToList();
            }
            else
            {
                var url = string.Format(Settings.WebAPIUrlFormatMap[Locale],
                    dateTime_first.Year,
                    dateTime_first.Month.PaddingZero(),
                    dateTime_first.Day.PaddingZero(),
                    dateTime_last.Year,
                    dateTime_last.Month.PaddingZero(),
                    dateTime_last.Day.PaddingZero());
                return processFrom_zhCN_zhTW(url);
            }
        }

        private List<RawEvent> processFrom_enUS(string url)
        {
            var dom = CQ.CreateFromUrl(url);
            return processFrom_enUS(dom);
        }

        private List<RawEvent> processFrom_enUS(CQ dom)
        {
            var eventGroups_daily = dom["table.tab-pane"];
            var list = new List<RawEvent>();

            foreach (var eventGroup in eventGroups_daily.Select(x => x.Cq()))
            {
                var datetimeInformation = eventGroup.Find("tr:first-child").Text().Trim();
                var events = eventGroup.Find("tr.event");
                RawEvent re = null;

                foreach (var e in events.Select(x => x.Cq()))
                {
                    var fields = e.Find("td.calendar-td");
                    foreach (var field_withIndex in fields.Select((x, i) => new { Value = x.Cq(), Index = i }))
                    {
                        if (field_withIndex.Index < 8)
                        {
                            var propertyName = Settings.ColumnMap[Locale][field_withIndex.Index];
                            if (!string.IsNullOrWhiteSpace(propertyName))
                            {
                                var text = field_withIndex.Value.Text().Trim();

                                if (field_withIndex.Index == 0)
                                {
                                    var m = Regex.Match(text, @"^\d{2}:\d{2}$");
                                    if (m.Success || string.IsNullOrWhiteSpace(text))
                                    {
                                        re = new RawEvent(Locale, datetimeInformation);
                                    }
                                    else
                                    {
                                        re.Memo = text;
                                        continue;
                                    }
                                }

                                if (field_withIndex.Index < 7)
                                {
                                    if (propertyName == "Currency")
                                    {
                                        text = field_withIndex.Value.Find("div").Attr("data-filter");
                                    }

                                    typeof(RawEvent).GetProperty(propertyName).SetValue(re, text);
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

        private List<RawEvent> processFrom_zhCN_zhTW(string url)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var dom = CQ.CreateFromUrl(url);
            return processFrom_zhCN_zhTW(dom);
        }

        private List<RawEvent> processFrom_zhCN_zhTW(CQ dom)
        {
            var eventGroups_daily = dom["tbody"];
            var list = new List<RawEvent>();

            foreach (var eventGroup in eventGroups_daily.Select(x => x.Cq()))
            {
                var datetimeInformation = eventGroup.Find("tr:first-child").Text().Trim();
                var events = eventGroup.Find("tr.dfx-calendar-item, tr.dfx-event-detail");
                RawEvent re = null;

                foreach (var e in events.Select(x => x.Cq()))
                {
                    var fields = e.Find("td");
                    foreach (var field_withIndex in fields.Select((x, i) => new { Value = x.Cq(), Index = i }))
                    {
                        if (field_withIndex.Index < 9)
                        {
                            var propertyName = Settings.ColumnMap[Locale][field_withIndex.Index];
                            if (!string.IsNullOrWhiteSpace(propertyName))
                            {
                                var text = field_withIndex.Value.Text().Trim();
                                if (field_withIndex.Index == 0)
                                {
                                    var m = Regex.Match(text, @"^\d{2}:\d{2}$");
                                    if (m.Success || string.IsNullOrWhiteSpace(text))
                                    {
                                        re = new RawEvent(Locale, datetimeInformation);
                                    }
                                    else
                                    {
                                        re.Memo = text;
                                        continue;
                                    }
                                }

                                if (field_withIndex.Index < 8)
                                {
                                    typeof(RawEvent).GetProperty(propertyName).SetValue(re, text);
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
