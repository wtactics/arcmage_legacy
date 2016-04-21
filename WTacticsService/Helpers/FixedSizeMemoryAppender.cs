using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Appender;
using log4net.Core;

namespace WTacticsService.Helpers
{
    public class FixedSizeMemoryAppender : MemoryAppender
    {
        private const int MaxEvents =  500;

        private Queue<LoggingEvent> LoggingEvents = new Queue<LoggingEvent>();

        private object lockLock = new object();

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (lockLock)
            {
                if (LoggingEvents.Count >= MaxEvents)
                {
                    LoggingEvents.Dequeue();
                }
                LoggingEvents.Enqueue(loggingEvent);
            }
        }

        public override void Clear()
        {
            lock (lockLock)
            {
                LoggingEvents.Clear();
            }
        }

        public override LoggingEvent[] PopAllEvents()
        {
            lock (lockLock)
            {
                var result = GetEvents();
                Clear();
                return result;
            }
        }

        public override LoggingEvent[] GetEvents()
        {
            lock (lockLock)
            {
                return LoggingEvents.Reverse().ToArray();
            }
        }
    }
}