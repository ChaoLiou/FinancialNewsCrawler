using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinCalendarParser
{
    public class RawEvent
    {
        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                var m = Regex.Match(value, @"^(?<y>\d{4}).(?<m>\d{2}).(?<d>\d{2}).");
                if (m.Success)
                {
                    _date = string.Format("{0}-{1}-{2}", m.Groups["y"].Value, m.Groups["m"].Value, m.Groups["d"].Value);
                }
                else
                {
                    DateTime dateTime;
                    if (DateTime.TryParse(value, out dateTime))
                    {
                        _date = dateTime.ToString(@"yyyy-MM-dd");
                    }
                    else
                    {
                        _date = value;
                    }
                }
            }
        }
        public string Time { get; set; }
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
        private string _date;
        private string _importance;
        public override string ToString()
        {
            Memo = string.IsNullOrWhiteSpace(Memo) ? string.Empty : Memo;
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\"", Date, Time, Currency, Description, Importance, Actual, Forecast, Previous, Memo);
        }

        private static string formatText(string text)
        {
            return "<span style='color:#000000'>" + (string.IsNullOrWhiteSpace(text) ? string.Empty : text) + "</span>";
        }

        private static Dictionary<string, string> importanceMap_EnUS = new Dictionary<string, string>()
        {
            {"High", "高"}, {"Medium", "中"}, {"Low", "低"}, {"", ""}
        };
    }
}
