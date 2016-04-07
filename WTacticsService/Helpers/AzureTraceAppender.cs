using System.Diagnostics;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Core;

namespace WTacticsService.Helpers
{
    /// <summary>
    /// Append the logging to the .NET Trace class using the correct level.
    /// The default Log4Net TraceAppender will trace everything on debug-level.
    /// http://stackoverflow.com/questions/11802663/log4net-traceappender-only-logs-messages-with-level-verbose-when-using-windows
    /// </summary>
    public sealed class AzureTraceAppender : TraceAppender
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(AzureTraceAppender));

        private static bool _flushWarningLogged = false;
        protected override void Append(LoggingEvent loggingEvent)
        {
            var level = loggingEvent.Level;
            var message = RenderLoggingEvent(loggingEvent);

            message = message.Replace('\"', '~');   // Azure escapes all double quotes. This makes it hard to read them back. We avoid this character altogheter, our reader will convert it back...

            if (level >= Level.Error)
            {
                Trace.TraceError(message);
            }
            else if (level >= Level.Warn)
            {
                Trace.TraceWarning(message);
            }
            else if (level >= Level.Info)
            {
                Trace.TraceInformation(message);
            }
            else
            {
                Trace.WriteLine(message);
            }

            if (ImmediateFlush)
            {
                Trace.Flush();

                // Most of the time ImmediateFlush is a bad idea when storing the logging on another server! Let's warn the user...
                if (!_flushWarningLogged)
                {
                    _flushWarningLogged = true;

                    // We cannot log from here - it would result in a recursive read lock exception.
                    // We log from the threadpool instead.
                    Task.Run(() => _log.Warn("ImmediateFlush is set on an AzureTraceAppender. This will make the logging very slow. It should only be enabled to diagnose very specific crashes. Add <immediateFlush>false</immediateFlush> to your log4net config to fix this."));
                }
            }
        }
    }
}
