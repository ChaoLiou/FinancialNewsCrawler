using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FinCalendarParser
{
    public class FX168Event
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Importance { get; set; }
        public string Previous { get; set; }
        public string Forecast { get; set; }
        public string Actual { get; set; }
        public string Revised { get; set; }
        public string DataTypeName { get; set; }
        public string Type { get; set; }

        private readonly static Dictionary<string, string> WEIGHTINESS_DICT = new Dictionary<string, string>()
        {
            { "", "" },
            { "0", "" },
            { "1", "低" },
            { "2", "中" },
            { "3", "高" },
        };
        public FX168Event(FinancialCalendarData fcd)
        {
            var noTimeData = string.IsNullOrWhiteSpace(fcd.Time) || fcd.Time == "--";
            var dateTime = DateTime.Parse(noTimeData ? $"{fcd.Date}" : $"{fcd.Date} {fcd.Time}");
            Date = dateTime.ToString("yyyy/MM/dd");
            Time = noTimeData ? fcd.Time : dateTime.ToString("HH:mm");
            Currency = fcd.CountryName;
            Description = fcd.Content;
            Importance = WEIGHTINESS_DICT[fcd.Weightiness];
            Previous = fcd.Previous;
            Forecast = fcd.Predict;
            Actual = fcd.CurrentValue;
            Revised = fcd.revisedCorrect;
            DataTypeName = fcd.DataTypeName;
            Type = "財經日曆";
        }

        public FX168Event(SubEvent se, string type)
        {
            var noTimeData = string.IsNullOrWhiteSpace(se.FinancialTime);
            var dateTime = DateTime.Parse(noTimeData ? $"{se.FinancialDate}" : $"{se.FinancialDate} {se.FinancialTime}");
            Date = dateTime.ToString("yyyy/MM/dd");
            Time = noTimeData ? "----" : dateTime.ToString("HH:mm");
            Currency = se.Area;
            Description = se.FinancialEvent;
            Importance = WEIGHTINESS_DICT[se.weightiness];
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\"", Date, Time, Currency, Description, Importance, Previous, Forecast, Actual, Revised, DataTypeName, Type);
        }
    }

    public class FX168Raw
    {
        public List<@List> @List { get; set; }
    }

    public class @List
    {
        public List<FinancialCalendarData> FinancialCalendarData { get; set; }
        public List<CentralBankNews> CentralBankNews { get; set; }
        public List<FinancialEvent> FinancialEvent { get; set; }
        public List<FinancialEvent> HolidayReport { get; set; }
    }

    public class FinancialCalendarData
    {
        public string id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Weightiness { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Content { get; set; }
        public string ColumnCode { get; set; }
        public string Previous { get; set; }
        public string Predict { get; set; }
        public string CurrentValue { get; set; }
        public string Revised { get; set; }
        public string DataType { get; set; }
        public string DataTypeName { get; set; }
        public string AD_GGZ { get; set; }
        public string AD_LD { get; set; }
        public string AD_LK { get; set; }
        public string ClumnName { get; set; }
        public string DataFrequency { get; set; }
        public string revisedCorrect { get; set; }
    }

    public class SubEvent
    {
        public string FinancialDate { get; set; }
        public string FinancialTime { get; set; }
        public string Area { get; set; }
        public string FinancialEvent { get; set; }
        public string weightiness { get; set; }
        public string Country { get; set; }
    }

    public class CentralBankNews : SubEvent
    {
    }

    public class FinancialEvent : SubEvent
    {

    }

    public class HolidayReport : SubEvent
    {

    }
}
