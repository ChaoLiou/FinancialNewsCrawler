using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCalendarParser
{
    public class Settings
    {
        public static Dictionary<Locale, string> WebAPIUrlFormatMap = new Dictionary<Locale, string>()
        {
            {Locale.ZhCN, "https://www.dailyfx.com.hk/calendar/getData/{0}-{1}-{2}/{3}-{4}-{5}"},
            {Locale.ZhTW, "https://www.dailyfx.com.hk/cn/calendar/getData/{0}-{1}-{2}/{3}-{4}-{5}"},
            {Locale.EnUS, "https://www.dailyfx.com/calendar?previous=true&week={0}/{1}{2}"}
        };

        public static Dictionary<Locale, string> LocaleAliasMap = new Dictionary<Locale, string>()
        {
            {Locale.ZhTW, "cn"},
            {Locale.ZhCN, "cnc"},
            {Locale.EnUS, "en"}
        };

        public static Dictionary<Locale, string[]> ColumnMap = new Dictionary<Locale, string[]>()
        {
            {Locale.ZhTW, new string[] { "Time", "Currency", "Description", "", "Importance", "Actual", "Forecast", "Previous", "Memo" }},
            {Locale.ZhCN, new string[] { "Time", "Currency", "Description", "", "Importance", "Actual", "Forecast", "Previous", "Memo" }},
            {Locale.EnUS, new string[] { "Time", "Currency", "Description", "Importance", "Actual", "Forecast", "Previous", "Memo" }}
        };
    }

    public enum Locale
    {
        ZhTW,
        ZhCN,
        EnUS
    }

    public enum PeriodType
    {
        Day,
        Week,
        Month
    }
}
