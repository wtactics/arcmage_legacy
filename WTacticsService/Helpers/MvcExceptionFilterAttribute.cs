using System.Web.Mvc;
using log4net;

namespace WTacticsService.Helpers
{
    /// <summary>
    /// For ASP.NET MVC: makes sure that all exceptions are logged.
    /// </summary>
    public class MvcExceptionFilterAttribute : HandleErrorAttribute
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MvcExceptionFilterAttribute));

        public override void OnException(ExceptionContext filterContext)
        {
            _log.Error(filterContext.Exception);
        }
    }
}