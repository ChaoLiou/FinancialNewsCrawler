using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FinCalendarParser
{
    public class FX168Parser
    {
        private static string _APIUrlFT = "https://financialcalendar.fx168api.com/FinancialCalendarNew/IFinancialCalendarData.ashx?user=FX168&beginDate={0}-{1}-{2}&endDate={3}-{4}-{5}";

        public List<FX168Event> Process(DateTime dateTime, PeriodType periodType)
        {
            DateTime? dtStart = null, dtEnd = null;
            switch (periodType)
            {
                case PeriodType.Week:
                    dtStart = dateTime.StartOfWeek(DayOfWeek.Monday);
                    dtEnd = dateTime.StartOfWeek(DayOfWeek.Monday).AddDays(6);
                    break;
                case PeriodType.Month:
                    var daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                    dtStart = new DateTime(dateTime.Year, dateTime.Month, 1);
                    dtEnd = new DateTime(dateTime.Year, dateTime.Month, daysInMonth);
                    break;
                default:
                    break;
            }

            if (dtStart.HasValue && dtEnd.HasValue)
            {
                return Transform(Process(dtStart.Value, dtEnd.Value));
            }
            else
            {
                return null;
            }
        }

        private FX168Raw Process(DateTime dtStart, DateTime dtEnd)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.Referer, "https://datainfo.fx168.com/calendar.shtml");
                var url = string.Format(_APIUrlFT, dtStart.Year, dtStart.Month.PaddingZero(), dtStart.Day.PaddingZero(), dtEnd.Year, dtEnd.Month.PaddingZero(), dtEnd.Day.PaddingZero());
                var data = wc.DownloadData(url);
                return JsonConvert.DeserializeObject<FX168Raw>(Encoding.UTF8.GetString(data));
            }
        }

        public List<FX168Event> Transform(FX168Raw raw)
        {
            var data = raw.List.FirstOrDefault();
            var events = new List<FX168Event>();
            if (data != null)
            {
                if (data.FinancialCalendarData != null)
                {
                    data.FinancialCalendarData.ForEach(fcd => events.Add(new FX168Event(fcd)));
                }
                if (data.FinancialEvent != null)
                {
                    data.FinancialEvent.ForEach(se => events.Add(new FX168Event(se, "財經事件")));
                }
                if (data.CentralBankNews != null)
                {
                    data.CentralBankNews.ForEach(se => events.Add(new FX168Event(se, "央行動態")));
                }
                if (data.HolidayReport != null)
                {
                    data.HolidayReport.ForEach(se => events.Add(new FX168Event(se, "假日預告")));
                }
            }
            return events;
        }
    }
}
