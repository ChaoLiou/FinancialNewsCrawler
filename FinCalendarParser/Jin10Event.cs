using System;

namespace FinCalendarParser
{
    public class Jin10Event
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public int Importance { get; set; }
        public string Previous { get; set; }
        public string Forecast { get; set; }
        public string Actual { get; set; }
        public string Revised { get; set; }
        public int Affect { get; set; }
        public Jin10Event(Jin10RawEvent re)
        {
            if (!string.IsNullOrWhiteSpace(re.pub_time))
            {
                var dateTime = DateTime.Parse(re.pub_time);
                Date = dateTime.ToString("yyyy/MM/dd");
                Time = dateTime.ToString("HH:mm");
                Currency = re.country;
                Description = re.time_period + re.name + (!string.IsNullOrWhiteSpace(re.unit) ? "(" + re.unit + ")": "");
                Importance = re.star;
                Previous = re.previous;
                Forecast = re.consensus;
                Actual = re.actual;
                Revised = re.revised;
                Affect = re.affect;
            }
        }
        public override string ToString()
        {
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\"", Date, Time, Currency, Description, Importance, Previous, Forecast, Actual, Revised, Affect);
        }
    }

    public class Jin10RawEvent
    {
        public int id { get; set; }
        public int affect { get; set; }
        public string country { get; set; }
        public string actual { get; set; }
        public string consensus { get; set; }
        public string unit { get; set; }
        public string revised { get; set; }
        public int star { get; set; }
        public string previous { get; set; }
        public string name { get; set; }
        public string pub_time { get; set; }
        public int indicator_id { get; set; }
        public string time_period { get; set; }
    }
}
