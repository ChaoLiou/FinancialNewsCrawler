using FinCalendarParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FinCalendarCrawler
{
    /// <summary>
    /// --src=dailyfx --date=2019-01-01 --period=week
    /// --src=dailyfx --date=2019-01-01 --period=month
    /// --src=jin10 --date=2019-01-01 --period=week
    /// --src=jin10 --date=2019-01-01 --period=month
    /// --src=fx168 --date=2019-01-01 --period=week
    /// --src=fx168 --date=2019-01-01 --period=month
    /// </summary>
    public class Args
    {
        public string Source { get; set; }
        public string Date { get; set; }
        public string Period { get; set; }
        public bool Translate { get; set; }
        public static bool Parsing(string[] args, out Args argsObj)
        {
            argsObj = new Args()
            {
                Source = Filter(args, "--src="),
                Date = Filter(args, "--date="),
                Period = Filter(args, "--period="),
                Translate = args.Any(x => x == "--translate")
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
                "[--{0}={{period}}]",
                "[--{0}]"
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
        public static bool _translate = false;
        static void Main(string[] args)
        {
            if (Args.Parsing(args, out Args argsObj))
            {
                var dateTime = argsObj.Date != null ?
                    DateTime.Parse(argsObj.Date) :
                    DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                var periodType = argsObj.Period.ParseToPeriod();
                var sourceType = argsObj.Source.ParseToSource();
                _translate = argsObj.Translate;

                switch (sourceType)
                {
                    case SourceType.DailyFX:
                        Process<DailyFXParser, DailyFXEvent>(dateTime, periodType, Locale.EnUS);
                        Process<DailyFXParser, DailyFXEvent>(dateTime, periodType, Locale.ZhCN);
                        Process<DailyFXParser, DailyFXEvent>(dateTime, periodType, Locale.ZhTW);
                        break;
                    case SourceType.Jin10:
                        Process<Jin10Parser, Jin10Event>(dateTime, periodType);
                        break;
                    case SourceType.FX168:
                        Process<FX168Parser, FX168Event>(dateTime, periodType);
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

        private static void Process<T1, T2>(DateTime dateTime, PeriodType periodType, Locale locale)
        {
            var parser = Activator.CreateInstance(typeof(T1), locale);
            Process<T1, T2>(parser, dateTime, periodType);
        }

        private static void Process<T1, T2>(DateTime dateTime, PeriodType periodType)
        {
            var parser = Activator.CreateInstance<T1>();
            Process<T1, T2>(parser, dateTime, periodType);
        }

        private static void Process<T1, T2>(object parser, DateTime dateTime, PeriodType periodType)
        {
            var processMethod = typeof(T1).GetMethod("Process");
            var list = ((List<T2>)processMethod.Invoke(parser, new object[] { dateTime, periodType }));
            var sourceName = typeof(T1).Name.Replace("Parser", "");
            if (_translate)
            {
                var mapDirectoryName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "0(1)(2)", sourceName);
                var map0FileName = Path.Combine(mapDirectoryName, "0.csv");
                var map1FileName = Path.Combine(mapDirectoryName, "1.csv");
                var map2FileName = Path.Combine(mapDirectoryName, "2.csv");
                if (File.Exists(map0FileName) && File.Exists(map1FileName) && File.Exists(map2FileName))
                {
                    var map0Content = File.ReadAllText(map0FileName);
                    var map1Content = File.ReadAllText(map1FileName);
                    var map2Content = File.ReadAllText(map2FileName);
                    var maps = new Dictionary<int, Dictionary<string, string>>()
                    {
                        { 0, map0Content.ToMap() },
                        { 1, map1Content.ToMap() },
                        { 2, map2Content.ToMap() }
                    };
                    list = TranslateAllDesc(list, maps).ToList();
                }
            }
            Output(list, dateTime, periodType);
        }

        private static IEnumerable<T> TranslateAllDesc<T>(IEnumerable<T> list, Dictionary<int, Dictionary<string, string>> maps)
        {
            return list.Select(e => TranslateDesc(e, maps[0]))
                .Select(e => TranslateDesc(e, maps[1], putBehind: true))
                .Select(e => TranslateDesc(e, maps[2], putBehind: true));
        }

        private static T TranslateDesc<T>(T @event, Dictionary<string, string> map, bool putBehind = false)
        {
            var desc = (string)typeof(T).GetProperty("Description").GetValue(@event);
            map.ToList().ForEach(x =>
            {
                if (desc.Contains(x.Key))
                {
                    if (putBehind)
                    {
                        desc = desc.Replace(x.Key, string.Empty);
                        desc += x.Value;
                    }
                    else
                    {
                        desc = desc.Replace(x.Key, x.Value);
                    }
                }
            });
            typeof(T).GetProperty("Description").SetValue(@event, desc);
            return @event;
        }

        private static void Output<T>(List<T> list, DateTime dateTime, PeriodType periodType)
        {
            var localePropName = "Locale";
            var excludes = new string[] { localePropName };
            var headers = typeof(T).GetProperties().Select(pi => pi.Name);
            var hasLocaleProp = headers.Any(h => h == localePropName);
            var headerRow = string.Join(",", headers.Where(h => !excludes.Contains(h)));

            var contents = new List<string>() { headerRow };
            contents.AddRange(list.Select(x => x.ToString()));

            var fileNamePrefix = typeof(T).Name.Replace("Event", "");
            var fileName = $"{fileNamePrefix}_{dateTime.ToString("yyyy-MM-dd")}_{periodType}";
            if (hasLocaleProp)
            {
                var locale = typeof(T).GetProperty(localePropName).GetValue(list[0]);
                fileName = $"{fileName}_{locale}";
            }
            if (_translate)
            {
                fileName = $"{fileName}_translate";
            }
            var destination = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{fileName}.csv");
            File.WriteAllLines(destination, contents, Encoding.UTF8);
        }
    }
}