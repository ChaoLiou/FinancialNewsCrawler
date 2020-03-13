using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinCalendarParser
{
    public class FX168Parser
    {
        private static string _APIUrlFT = "https://dataapi.2rich.net/InterfaceCollect/default.aspx?Code=fx168&bCode=IFinancialCalendarData{0}-{1}-{2}&succ_callback=CallbackFinanceListDataByDate&_={3}";
        public List<FX168Event> Process(DateTime dateTime, PeriodType periodType)
        {
            DateTime? dtStart = null, dtEnd = null;
            switch (periodType)
            {
                case PeriodType.Week:
                    dtStart = dateTime.StartOfWeek(DayOfWeek.Monday);
                    dtEnd = dateTime.StartOfWeek(DayOfWeek.Monday).AddDays(6);
                    break;
                case PeriodType.Month:
                    var daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
                    dtStart = new DateTime(dateTime.Year, dateTime.Month, 1);
                    dtEnd = new DateTime(dateTime.Year, dateTime.Month, daysInMonth);
                    break;
                default:
                    break;
            }

            if (dtStart.HasValue && dtEnd.HasValue)
            {
                DateTime date = dtStart.Value;
                var list = new List<FX168Event>();
                while (date.CompareTo(dtEnd.Value) <= 0)
                {
                    list.AddRange(Transform(Process(date)));
                    date = date.AddDays(1);
                }
                return list;
            }
            else
            {
                return null;
            }
        }

        private FX168Raw Process(DateTime dt)
        {
            using (var wc = new WebClient())
            {
                Console.WriteLine("Crawling on {0}...", dt.ToString(@"yyyy-MM-dd"));
                wc.Headers.Add(HttpRequestHeader.Referer, "https://datainfo.fx168.com/calendar.shtml");
                wc.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                var url = string.Format(_APIUrlFT, dt.Year, dt.Month.PaddingZero(), dt.Day.PaddingZero(), DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                byte[] data = null;
                int count = 0;
                while(true)
                {
                    try
                    {
                        Console.WriteLine("-> #{0} Time", ++count);
                        data = wc.DownloadData(url);
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }
                
                data = DecompressGZip(data);
                var match = Regex.Match(Encoding.UTF8.GetString(data), @"^CallbackFinanceListDataByDate\((?<data>.*?)\)$");
                return match.Success ? JsonConvert.DeserializeObject<FX168Raw>(match.Groups["data"].Value) : null;
            }
        }

        public List<FX168Event> Transform(FX168Raw raw)
        {
            var data = raw.List.FirstOrDefault();
            var events = new List<FX168Event>();
            if (data != null)
            {
                if (data.FinancialCalendarData != null)
                {
                    data.FinancialCalendarData.ForEach(fcd => events.Add(new FX168Event(fcd)));
                }
                if (data.FinancialEvent != null)
                {
                    data.FinancialEvent.ForEach(se => events.Add(new FX168Event(se, "財經事件")));
                }
                if (data.CentralBankNews != null)
                {
                    data.CentralBankNews.ForEach(se => events.Add(new FX168Event(se, "央行動態")));
                }
                if (data.HolidayReport != null)
                {
                    data.HolidayReport.ForEach(se => events.Add(new FX168Event(se, "假日預告")));
                }
            }
            return events;
        }

        public static byte[] DecompressGZip(byte[] bytesToDecompress)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(bytesToDecompress), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    int count;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memoryStream.Write(buffer, 0, count);
                        }
                    } while (count > 0);
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] CompressGZip(string input, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Unicode;
            byte[] bytes = encoding.GetBytes(input);
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    zipStream.Write(bytes, 0, bytes.Length);
                    return stream.ToArray();
                }
            }
        }
    }
}
