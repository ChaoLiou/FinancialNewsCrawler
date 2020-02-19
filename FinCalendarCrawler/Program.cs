using FinCalendarParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinCalendarCrawler
{
    /// <summary>
    /// --source=dailyfx --date=2019-01-01 --locale=zh-tw --period=week
    /// --source=dailyfx --date=2019-01-01 --locale=zh-tw --period=month
    /// --source=dailyfx --date=2019-01-01 --locale=zh-cn --period=week
    /// --source=dailyfx --date=2019-01-01 --locale=zh-cn --period=month
    /// --source=dailyfx --date=2019-01-01 --locale=en-us --period=week
    /// --source=dailyfx --date=2019-01-01 --locale=en-us --period=month
    /// --source=jin10 --date=2019-01-01 --period=week
    /// --source=jin10 --date=2019-01-01 --period=month
    /// --source=fx168 --date=2019-01-01 --period=week
    /// --source=fx168 --date=2019-01-01 --period=month
    /// </summary>
    public class Args
    {
        public string Source { get; set; }
        public string Date { get; set; }
        public string Period { get; set; }
        public static bool Parsing(string[] args, out Args argsObj)
        {
            argsObj = new Args()
            {
                Source = Filter(args, "--src="),
                Date = Filter(args, "--date="),
                Period = Filter(args, "--period=")
            };

            return !string.IsNullOrWhiteSpace(argsObj.Source);
        }

        private static string Filter(string[] args, string contains)
        {
            return args.Where(arg => arg.ToLower().StartsWith(contains))
                .Select(x => x.Replace(contains, string.Empty)).FirstOrDefault();
        }

        public static string Helper()
        {
            var usagesFT = new string[]
            {
                "[--{0}={{source}}]",
                "[--{0}=2018-01-01|{{today}}]",
                "[--{0}={{period}}]"
            };
            return "Helper: FinCalendarCrawler.exe " + string.Join(" ",
                typeof(Args).GetProperties()
                .Select((x, i) => string.Format(usagesFT[i], x.Name.ToLower()))) +
                "\r\n*period:[week,month], optional, default is week" +
                "\r\n*source:[dailyfx,jin10], required";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (Args.Parsing(args, out Args argsObj))
            {
                var dateTime = argsObj.Date != null ?
                    DateTime.Parse(argsObj.Date) :
                    DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                var periodType = argsObj.Period.ParseToPeriod();
                var sourceType = argsObj.Source.ParseToSource();

                switch (sourceType)
                {
                    case SourceType.DailyFX:
                        ProcessDailyFX(dateTime, periodType, Locale.EnUS);
                        ProcessDailyFX(dateTime, periodType, Locale.ZhCN);
                        ProcessDailyFX(dateTime, periodType, Locale.ZhTW);
                        break;
                    case SourceType.Jin10:
                        ProcessJin10(dateTime, periodType);
                        break;
                    case SourceType.FX168:
                        ProcessFX168(dateTime, periodType);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine(Args.Helper());
            }
        }

        private static void ProcessDailyFX(DateTime dateTime, PeriodType periodType, Locale locale)
        {
            var parser = new DailyFXParser(locale);
            var list = parser.Process(dateTime, periodType);
            OutputDailyFXCSV(list, dateTime, locale, periodType);
        }

        private static void ProcessJin10(DateTime dateTime, PeriodType periodType)
        {
            var parser = new Jin10Parser();
            var list = parser.Process(dateTime, periodType);
            OutputJin10CSV(list, dateTime, periodType);
        }

        private static void ProcessFX168(DateTime dateTime, PeriodType periodType)
        {
            var parser = new FX168Parser();
            var list = parser.Process(dateTime, periodType);
            OutputFX168CSV(list, dateTime, periodType);
        }

        private static void OutputJin10CSV(List<Jin10Event> list, DateTime dateTime, PeriodType periodType)
        {
            var csvContents = new List<string>() { "Date,Time,Currency,Description,Importance,Previous,Forecast,Actual,Revised,Affect" };
            csvContents.AddRange(list.Select(x => x.ToString()));
            var fileName = "Jin10_" + dateTime.ToString("yyyy-MM-dd") + "_" + periodType + ".csv";
            var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            File.WriteAllLines(destination, csvContents, Encoding.UTF8);
        }

        private static void OutputDailyFXCSV(List<DailyFXEvent> list, DateTime dateTime, Locale locale, PeriodType periodType)
        {
            var csvContents = new List<string>() { "Date,Time,Currency,Description,Importance,Previous,Forecast,Actual,Memo" };
            csvContents.AddRange(list.Select(x => x.ToString()));
            var fileName = "DailyFX_" + dateTime.ToString("yyyy-MM-dd") + "_" + periodType + "." + locale + ".csv";
            var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            File.WriteAllLines(destination, csvContents, Encoding.UTF8);
        }

        private static void OutputFX168CSV(List<FX168Event> list, DateTime dateTime, PeriodType periodType)
        {
            var csvContents = new List<string>() { "Date,Time,Currency,Description,Importance,Previous,Forecast,Actual,Revised,DataTypeName,Type" };
            csvContents.AddRange(list.Select(x => x.ToString()));
            var fileName = "FX168_" + dateTime.ToString("yyyy-MM-dd") + "_" + periodType + ".csv";
            var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            File.WriteAllLines(destination, csvContents, Encoding.UTF8);
        }
    }
}