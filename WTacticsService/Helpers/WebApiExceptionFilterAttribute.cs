using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using log4net;

namespace WTacticsService.Helpers
{
    /// <summary>
    /// For ASP.NET Web api: makes sure that all exceptions are logged.
    /// </summary>
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(WebApiExceptionFilterAttribute));

        public override void OnException(HttpActionExecutedContext context)
        {
            _log.Error(context.Exception);

            if (context.Exception is HttpResponseException)
            {
                // No need to wrap the exception
                throw context.Exception;
            }

            string message;
            if (context.Exception is ApplicationException)  // Those types of exceptions are specifically meant to be shown to the client (by convention in this project). However we don't pass stacktraces
            {
                message = context.Exception.Message;
            }
            else
            {
                message = "An error occurred, please try again or contact the administrator.";
            }

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(message),
                ReasonPhrase = "An error occurred, please try again or contact the administrator."
            });
        }
    }
}