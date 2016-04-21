using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using WTacticsLibrary.Model;
using WTacticsService.Helpers;

namespace WTacticsService.Api
{
    [Authorize]
    public class LogController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            var hierarchy = LogManager.GetRepository() as Hierarchy;
            if (hierarchy != null)
            {
                var appender = hierarchy.Root.GetAppender("FixedSizeMemoryAppender") as FixedSizeMemoryAppender;
                var events = appender.GetEvents().Select(GetEvent);

                return Request.CreateResponse(new ResultList<LogEvent>(events.ToList()));
            }
            return Request.CreateResponse();
        }

        private LogEvent GetEvent(LoggingEvent loggingEvent)
        {
            return new LogEvent
            {
                RenderedMessage = loggingEvent.RenderedMessage,
                LoggerName = loggingEvent.LoggerName,
                TimeStamp = loggingEvent.TimeStamp,
                LevelValue = loggingEvent.Level.Value,
                LevelName = loggingEvent.Level.Name,
                Exception = loggingEvent.GetExceptionString()

            };
            
        }
    }
}
