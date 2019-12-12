using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCalendarParser
{
    public class DailyFXRaw_en
    {
        public string importance { get; set; }
        public string title { get; set; }
        public string previous { get; set; }
        public string forecast { get; set; }
        public string currency { get; set; }
        public string actual { get; set; }
        public bool isAllDayEvent { get; set; }
        public int eventDate { get; set; }
        public string comment { get; set; }
        public string better { get; set; }
    }
}
