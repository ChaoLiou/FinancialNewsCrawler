using System;
using System.Collections.Generic;
using System.Linq;

namespace FinCalendarParser
{
    public static class ExtensionMethods
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime AddXDays(this DateTime dateTime, PeriodType periodType)
        {
            switch (periodType)
            {
                case PeriodType.Week:
                    dateTime = dateTime.AddDays(7);
                    break;
                case PeriodType.Month:
                    dateTime = dateTime.AddMonths(1).AddDays(-1);
                    break;
                default:
                    break;
            }

            return dateTime;
        }

        public static string PaddingZero(this int n, int length = 2)
        {
            return n.ToString().PadLeft(length, '0');
        }

        public static PeriodType ParseToPeriod(this string periodStr)
        {
            return !string.IsNullOrWhiteSpace(periodStr) ?
                (PeriodType)Enum.Parse(typeof(PeriodType), periodStr, true) :
                PeriodType.Week;
        }

        public static SourceType ParseToSource(this string sourceStr)
        {
            return !string.IsNullOrWhiteSpace(sourceStr) ?
                (SourceType)Enum.Parse(typeof(SourceType), sourceStr, true) :
                SourceType.DailyFX;
        }

        public static Dictionary<string, string> ToMap(this string content)
        {
            var map = new Dictionary<string, string>();
            content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(line =>
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        if (!map.ContainsKey(key))
                        {
                            map.Add(key, value);
                        }
                    }
                });
            return map;
        }
    }
}
