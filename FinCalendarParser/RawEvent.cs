using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FinCalendarParser
{
    public class RawEvent
    {
        public RawEvent(Locale locale, string date)
        {
            Locale = locale;
            var m = Regex.Match(date, @"^(?<y>\d{4}).(?<m>\d{2}).(?<d>\d{2}).");
            if (m.Success)
            {
                date = string.Format("{0}-{1}-{2}", m.Groups["y"].Value, m.Groups["m"].Value, m.Groups["d"].Value);
            }
            DateTime dateTime;
            if (DateTime.TryParse(date, out dateTime))
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
                return _time;
            }
            set
            {
                _time = value;
                if (!string.IsNullOrWhiteSpace(_time))
                {
                    var m = Regex.Match(_time, @"^(?<h>\d{2}):(?<m>\d{2})");
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
                return _description;
            }
            set
            {
                _description = formatText(string.IsNullOrWhiteSpace(Time) ? value : value.Replace(Time, string.Empty).Trim());
                _description = formatText(string.IsNullOrWhiteSpace(Currency) ? _description : _description.Replace(Currency.ToUpper(), string.Empty).Trim());
            }
        }
        public string Importance
        {
            get
            {
                return _importance;
            }
            set
            {
                _importance = value.Replace(" L", string.Empty).Replace(" M", string.Empty).Replace(" H", string.Empty).Trim();
                _importance = importanceMap_EnUS.Select(x => x.Value).Contains(_importance) ? _importance : importanceMap_EnUS[_importance];
            }
        }
        public string Actual
        {
            get
            {
                return _actual;
            }
            set
            {
                _actual = formatText(value);
            }
        }
        public string Forecast
        {
            get
            {
                return _forecast;
            }
            set
            {
                _forecast = formatText(value);
            }
        }
        public string Previous
        {
            get
            {
                return _previous;
            }
            set
            {
                _previous = formatText(value);
            }
        }
        public string Memo { get; set; }
        private string _forecast;
        private string _actual;
        private string _previous;
        private string _description;
        private string _importance;
        private string _time;
        public override string ToString()
        {
            Memo = string.IsNullOrWhiteSpace(Memo) ? string.Empty : Memo;
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"", Date.ToString("yyyy/MM/dd"), !string.IsNullOrWhiteSpace(Time) ? Date.ToString("HH:mm") : "", Currency, Description, Importance, Previous, Forecast, Actual, Memo);
        }

        private static string formatText(string text)
        {
            return (string.IsNullOrWhiteSpace(text) ? string.Empty : text);
        }

        private static Dictionary<string, string> importanceMap_EnUS = new Dictionary<string, string>()
        {
            {"High", "高"}, {"Medium", "中"}, {"Low", "低"}, {"", ""}
        };
    }
}
