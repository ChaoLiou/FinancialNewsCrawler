using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinCalendarParser
{
    public class DailyFXEvent
    {
        public DailyFXEvent(Locale locale, string dateTimeInfo)
        {
            Locale = locale;
            var m = Regex.Match(dateTimeInfo, @"^(?<y>\d{4}).(?<m>\d{2}).(?<d>\d{2}).");
            if (m.Success)
            {
                dateTimeInfo = string.Format("{0}-{1}-{2}", m.Groups["y"].Value, m.Groups["m"].Value, m.Groups["d"].Value);
            }
            DateTime dateTime;
            if (DateTime.TryParse(dateTimeInfo, out dateTime))
            {
                if (Locale == Locale.EnUS)
                {
                    dateTime = dateTime.AddHours(8);
                }
            }
            Date = dateTime;
        }
        public Locale Locale { get; set; }
        public DateTime Date { get; set; }
        public string Time
        {
            get
            {
                return _Time;
            }
            set
            {
                _Time = value;
                if (!string.IsNullOrWhiteSpace(_Time))
                {
                    var m = Regex.Match(_Time, @"^(?<h>\d{2}):(?<m>\d{2})");
                    if (m.Success)
                    {
                        var hour = int.Parse(m.Groups["h"].Value);
                        var minute = int.Parse(m.Groups["m"].Value);
                        Date = new DateTime(Date.Year, Date.Month, Date.Day, hour, minute, 0);
                        if (Locale == Locale.EnUS)
                        {
                            Date = Date.AddHours(8);
                        }
                    }
                }
            }
        }
        public string Currency { get; set; }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = _FormatText(string.IsNullOrWhiteSpace(Time) ? value : value.Replace(Time, string.Empty).Trim());
                _Description = _FormatText(string.IsNullOrWhiteSpace(Currency) ? _Description : _Description.Replace(Currency.ToUpper(), string.Empty).Trim());
            }
        }
        public string Importance
        {
            get
            {
                return _Importance;
            }
            set
            {
                _Importance = value.Replace(" L", string.Empty).Replace(" M", string.Empty).Replace(" H", string.Empty).Trim();
                _Importance = _ImportanceMapFromEnUSToZhTW.Select(x => x.Value).Contains(_Importance) ? _Importance : _ImportanceMapFromEnUSToZhTW[_Importance];
            }
        }
        public string Actual
        {
            get
            {
                return _Actual;
            }
            set
            {
                _Actual = _FormatText(value);
            }
        }
        public string Forecast
        {
            get
            {
                return _Forecast;
            }
            set
            {
                _Forecast = _FormatText(value);
            }
        }
        public string Previous
        {
            get
            {
                return _Previous;
            }
            set
            {
                _Previous = _FormatText(value);
            }
        }
        public string Memo { get; set; }
        private string _Forecast;
        private string _Actual;
        private string _Previous;
        private string _Description;
        private string _Importance;
        private string _Time;
        public override string ToString()
        {
            Memo = string.IsNullOrWhiteSpace(Memo) ? string.Empty : Memo;
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"", Date.ToString("yyyy/MM/dd"), !string.IsNullOrWhiteSpace(Time) ? Date.ToString("HH:mm") : "", Currency, Description, Importance, Previous, Forecast, Actual, Memo);
        }

        private static string _FormatText(string text)
        {
            return (string.IsNullOrWhiteSpace(text) ? string.Empty : text);
        }

        private static Dictionary<string, string> _ImportanceMapFromEnUSToZhTW = new Dictionary<string, string>()
        {
            {"High", "高"}, {"Medium", "中"}, {"Low", "低"}, {"", ""}
        };
    }
}
