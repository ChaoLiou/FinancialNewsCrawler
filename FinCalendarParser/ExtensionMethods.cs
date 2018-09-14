using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                case PeriodType.Day:
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

        public static Locale ParseToLocale(this string locale_str)
        {
            return (Locale)Enum.Parse(typeof(Locale), locale_str.Replace("-", string.Empty), true);
        }

        public static PeriodType ParseToPeriod(this string period_str)
        {
            return (PeriodType)Enum.Parse(typeof(PeriodType), period_str, true);
        }
    }
}
