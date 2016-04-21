using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Web.Http;
using log4net;

namespace WTacticsService.Helpers
{
    /// <summary>
    /// This class makes sure that all logging is done in a consistent matter.
    /// </summary>
    public static class LogExtensions
    {
        public static void LogRequest(this ILog log, HttpRequestBase request)
        {
            if (!log.IsDebugEnabled) return;

            var message = new StringBuilder();
            message.Append(request.RequestType);
            message.Append(" ");
            message.Append(request.Url);
            message.Append("\nHeaders:\n");
            foreach (var headerKey in request.Headers.AllKeys)
            {
                message.Append("[");
                message.Append(headerKey);
                message.Append("]=[");
                message.Append(request.Headers[headerKey]);
                message.Append("]\n");
            }
            message.Append("Cookies:\n");
            foreach (var cookieKey in request.Cookies.AllKeys)
            {
                message.Append("[");
                message.Append(cookieKey);
                message.Append("]=[");
                message.Append(request.Cookies[cookieKey].Value);
                message.Append("]\n");
            }
            log.Debug(message);
        }

        /// <summary>
        /// Log an exception. Depending on the type of exception, this method may try to extract additional information
        /// </summary>
        public static void LogException(this ILog log, Exception ex)
        {
            log.Error(GetExceptionDetails(ex));
        }

        /// <summary>
        /// Log an exception. Depending on the type of exception, this method may try to extract additional information
        /// </summary>
        public static void LogException(this ILog log, string message, Exception ex)
        {
            log.Error(message + "\n" + GetExceptionDetails(ex));
        }

        private static string GetExceptionDetails(Exception ex)
        {
            var validationException = ex as DbEntityValidationException;
            if (validationException != null)
            {
                var logging = new StringBuilder();
                logging.AppendLine(validationException.ToString());
                foreach (var validationError in validationException.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors) logging.AppendLine(error.PropertyName + " - " + error.ErrorMessage);
                }
                return logging.ToString();
            }

            var httpException = ex as HttpResponseException;
            if (httpException != null)
            {
                var logging = new StringBuilder();
                logging.AppendLine(httpException.ToString());
                if (httpException.Response != null)
                {
                    var str = httpException.Response.Content.ReadAsStringAsync().Result;    // TODO are we sure that this will not block?
                    logging.AppendLine(str);
                }
                return logging.ToString();
            }

            return ex.ToString();
        }

        /// <summary>
        /// Can be used to have a constistent logging of each method entry and exit.
        /// </summary>
        /// <returns>An object that you need to Dipose at the end of your method (ideally you should use the using(...) pattern). This will make sure that the end of the method
        /// is logged as well.</returns>
        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter[] parameters, [CallerMemberName]string methodName = null)
        {
            var now = DateTime.UtcNow;

            var formattedParams = string.Join(", ", parameters.Select(it => it.ToString()));
            var result = new LogMethodCookie(log, methodName, formattedParams, now);

            var message = "[Enter]=[" + methodName + "](" + formattedParams + ")";
            log.Debug(message);
            return result;
        }

        /* Unfortunately we cannot use params in combination with optional [CallerMemberName], so we have to provide all the overloads */
        public static LogMethodCookie LogMethodEnter(this ILog log, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[0], methodName);

        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter p1, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[] { p1 }, methodName);

        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter p1, MethodParameter p2, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[] { p1, p2 }, methodName);

        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter p1, MethodParameter p2, MethodParameter p3, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[] { p1, p2, p3 }, methodName);

        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter p1, MethodParameter p2, MethodParameter p3, MethodParameter p4, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[] { p1, p2, p3, p4 }, methodName);

        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter p1, MethodParameter p2, MethodParameter p3, MethodParameter p4, MethodParameter p5, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[] { p1, p2, p3, p4, p5 }, methodName);

        public static LogMethodCookie LogMethodEnter(this ILog log, MethodParameter p1, MethodParameter p2, MethodParameter p3, MethodParameter p4, MethodParameter p5, MethodParameter p6, [CallerMemberName]string methodName = null)
            => LogMethodEnter(log, new MethodParameter[] { p1, p2, p3, p4, p5, p6 }, methodName);

    }

    public sealed class LogMethodCookie : IDisposable
    {
        internal LogMethodCookie(ILog log, string methodName, string parameters, DateTime methodBegin)
        {
            _log = log;
            _methodName = methodName;
            _parameters = parameters;
            _methodBegin = methodBegin;
        }

        internal readonly ILog _log;
        internal readonly string _methodName;
        internal readonly string _parameters;
        internal readonly DateTime _methodBegin;

        public void Dispose()
        {
            var now = DateTime.UtcNow;
            var message = "[Exit]=[" + _methodName + "][Duration]=[" + (now - _methodBegin).TotalMilliseconds.ToString(CultureInfo.InvariantCulture) + "](" + _parameters + ")";
            _log.Debug(message);
        }
    }


    /// <summary>
    /// Used when logging the values of method parameters.
    /// </summary>
    public class MethodParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public MethodParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append('[');
            builder.Append(Name);
            builder.Append("]=[");
            builder.Append((Value != null) ? Value.ToString() : "null");
            builder.Append(']');
            return builder.ToString();
        }
    }

}