using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTacticsLibrary.Model
{
    public class LogEvent
    {
        public string RenderedMessage { get; set; }
        public string LoggerName { get; set; }
        public DateTime TimeStamp { get; set; }
        public int LevelValue { get; set; }
        public string LevelName { get; set; }
        public string Exception { get; set; }
    }
}
