using CsQuery;
using FinCalendarParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinCalendarCrawler
{
    public class Args
    {
        public string StartsWith { get; set; }
        public string Locale { get; set; }
        public string Period { get; set; }
        public bool DoUpdate { get; set; }

        public static bool Parsing(string[] args, out Args argsObj)
        {
            argsObj = new Args()
            {
                StartsWith = filter(args, "--startswith="),
                Locale = filter(args, "--locale="),
                Period = filter(args, "--period="),
                DoUpdate = filter(args, "--doupdate") != null
            };

            return !string.IsNullOrWhiteSpace(argsObj.StartsWith) && !string.IsNullOrWhiteSpace(argsObj.Locale);
        }

        private static string filter(string[] args, string contains)
        {
            return args.Where(arg => arg.ToLower().StartsWith(contains))
                .Select(x => x.Replace(contains, string.Empty)).FirstOrDefault();
        }

        public static string Helper()
        {
            var usages_format = new string[] { "--{0}=2018-01-01", "--{0}=zh-tw|zh-cn|en-us", "[--{0}=day|week|month]", "[--{0}]" };
            return "Helper: FinCalendarCrawler.exe " + string.Join(" ",
                typeof(Args).GetProperties()
                .Select((x, i) => string.Format(usages_format[i], x.Name)));
        }
    }

    class Program
    {
        private static string _apiUrl = ConfigurationManager.AppSettings["ApiUrl"];

        static void Main(string[] args)
        {
            Args argsObj;
            if (args.Any() && Args.Parsing(args, out argsObj))
            {
                var locale = argsObj.Locale.ParseToLocale();
                var periodType = PeriodType.Week;

                if (args.Length >= 3)
                {
                    periodType = argsObj.Period.ParseToPeriod();
                }

                var parser = new Parser(locale);
                var list = parser.Process(DateTime.Parse(argsObj.StartsWith), periodType);
                outputCSV(list, argsObj.StartsWith, locale, periodType);

                if (argsObj.DoUpdate)
                {
                    updateToProbisFinCalendarAPI(list, locale);
                }
            }
            else
            {
                Console.WriteLine(Args.Helper());
                Console.Read();
            }
        }

        private static void updateToProbisFinCalendarAPI(List<RawEvent> list, Locale locale)
        {
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    var json = JsonConvert.SerializeObject(list.Select((x, i) => new
                    {
                        id = i,
                        released_date = x.Date,
                        released_time = x.Time,
                        country = x.Currency,
                        @event = x.Description,
                        importance = x.Importance,
                        previous = x.Previous,
                        forecast = x.Forecast,
                        actual = x.Actual,
                        valid_date = "",
                        state = "new",
                        locale = Settings.LocaleAliasMap[locale]
                    }));
                    
                    wc.UploadString(_apiUrl, "data=" + Uri.EscapeDataString(json));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void outputCSV(List<RawEvent> list, string dateTime, Locale locale, PeriodType periodType)
        {
            var csvContents = new List<string>() { "Date,Time,Currency,Description,Importance,Actual,Forecast,Previous,Memo" };
            csvContents.AddRange(list.Select(x => x.ToString()));
            var fileName = "RawEvents_" + dateTime + "_" + periodType + "." + locale + ".csv";
            var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            File.WriteAllLines(destination, csvContents, Encoding.UTF8);
        }
    }
}