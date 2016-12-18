using System;
using Microsoft.Owin.Hosting;

namespace WTacticsGameService
{
    public static class ServiceController
    {
        private static IDisposable GameServiceWebApp { get; set; }

        public static void Start()
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://*:9091";
            GameServiceWebApp = WebApp.Start<GameServiceStartup>(url);
        }

        public static void Stop()
        {
            GameServiceWebApp?.Dispose();
        }
    }
}
