using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FinCalendarParser
{
    public class Jin10Parser
    {
        private static string _APIUrlFT = "https://cdn-rili.jin10.com/data/{0}/{1}{2}/economics.json";
        public List<Jin10Event> Process(DateTime dateTime, PeriodType periodType)
        {
            var dateTimesInRange = new List<DateTime>();
            switch (periodType)
            {
                case PeriodType.Week:
                    var dateTimeStartOfWeek = dateTime.StartOfWeek(DayOfWeek.Monday);
                    Enumerable.Range(0, 7).ToList().ForEach(index =>
                    {
                        dateTimesInRange.Add(dateTimeStartOfWeek.AddDays(index));
                    });
                    break;
                case PeriodType.Month:
                    var daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                    var dateTimeStartOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
                    Enumerable.Range(0, daysInMonth).ToList().ForEach(index =>
                    {
                        dateTimesInRange.Add(dateTimeStartOfMonth.AddDays(index));
                    });
                    break;
                default:
                    break;
            }
            return Process(dateTimesInRange).Select(re => new Jin10Event(re)).ToList();
        }

        private IEnumerable<Jin10RawEvent> Process(List<DateTime> dateTimes)
        {
            return dateTimes.SelectMany(dt => Process(dt));
        }

        private IEnumerable<Jin10RawEvent> Process(DateTime dateTime)
        {
            using (var wc = new WebClient())
            {
                var url = string.Format(_APIUrlFT, dateTime.Year, dateTime.Month.PaddingZero(), dateTime.Day.PaddingZero());
                var data = wc.DownloadData(url);
                return JsonConvert.DeserializeObject<List<Jin10RawEvent>>(Encoding.UTF8.GetString(data));
            }
        }
    }
}
