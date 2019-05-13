using FinCalendarParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FinCalendarCrawler
{
    public class Args
    {
        public string Date { get; set; }
        public string Locale { get; set; }
        public string Period { get; set; }
        public bool Help { get; set; }

        public static bool Parsing(string[] args, out Args argsObj)
        {
            argsObj = new Args()
            {
                Date = _Filter(args, "--date="),
                Locale = _Filter(args, "--locale="),
                Period = _Filter(args, "--period="),
                Help = _Contains(args, "--help")
            };

            return !string.IsNullOrWhiteSpace(argsObj.Date) && !string.IsNullOrWhiteSpace(argsObj.Locale);
        }

        private static bool _Contains(string[] args, string contains)
        {
            return args.Any(arg => arg.ToLower() == contains);
        }

        private static string _Filter(string[] args, string contains)
        {
            return args.Where(arg => arg.ToLower().StartsWith(contains))
                .Select(x => x.Replace(contains, string.Empty)).FirstOrDefault();
        }

        public static string Helper()
        {
            var usages_format = new string[] { "[--{0}=2018-01-01|{{today}}]", "[--{0}={{locale}}|{{all of locales}}]", "[--{0}={{period}}]", "[--{0}]" };
            return "Helper: FinCalendarCrawler.exe " + string.Join(" ",
                typeof(Args).GetProperties()
                .Select((x, i) => string.Format(usages_format[i], x.Name.ToLower()))) + "\r\n*locale:[zh-tw,zh-cn,en-us]\r\n*period:[day,week,month], default is week";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Args argsObj = null;
            if (Args.Parsing(args, out argsObj))
            {
                var locale = argsObj.Locale.ParseToLocale();
                var periodType = PeriodType.Week;

                if (argsObj.Period != null)
                {
                    periodType = argsObj.Period.ParseToPeriod();
                }
                
                process(DateTime.Parse(argsObj.Date), periodType, locale);
                
            }
            else if (argsObj.Help)
            {
                Console.WriteLine(Args.Helper());
            }
            else
            {
                var dateTime = argsObj.Date != null ? DateTime.Parse(argsObj.Date) : DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                var periodType = PeriodType.Week;

                if (argsObj.Period != null)
                {
                    periodType = argsObj.Period.ParseToPeriod();
                }

                if (argsObj.Locale != null)
                {
                    var locale = argsObj.Locale.ParseToLocale();
                    process(dateTime, periodType, locale);
                } else
                {
                    process(dateTime, periodType, Locale.EnUS);
                    process(dateTime, periodType, Locale.ZhCN);
                    process(dateTime, periodType, Locale.ZhTW);
                }
            }
        }

        private static void process(DateTime dateTime, PeriodType periodType, Locale locale)
        {
            var parser = new Parser(locale);
            var list = parser.Process(dateTime, periodType);
            _OutputCSV(list, dateTime, locale, periodType);
        }

        private static void _OutputCSV(List<RawEvent> list, DateTime dateTime, Locale locale, PeriodType periodType)
        {
            var csvContents = new List<string>() { "Date,Time,Currency,Description,Importance,Previous,Forecast,Actual,Memo" };
            csvContents.AddRange(list.Select(x => x.ToString()));
            var fileName = "Economics_" + dateTime.ToString("yyyy-MM-dd") + "_" + periodType + "." + locale + ".csv";
            var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            File.WriteAllLines(destination, csvContents, Encoding.UTF8);
        }
    }
}