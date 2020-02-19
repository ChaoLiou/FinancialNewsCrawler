using System.Collections.Generic;

namespace FinCalendarParser
{
    public class Config
    {
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
        Week,
        Month
    }

    public enum SourceType
    {
        DailyFX,
        Jin10,
        FX168
    }
}
