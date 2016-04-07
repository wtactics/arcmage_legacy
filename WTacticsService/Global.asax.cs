using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;


namespace WTacticsService
{
    

    public class WebApiApplication : System.Web.HttpApplication
    {
        private BackgroundJobServer _backgroundJobServer;

        protected void Application_Start()
        {


            var configuration = new WTacticsDAL.Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

            GeneralConfig.Config(System.Web.Http.GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("HangFireDB");
            _backgroundJobServer = new BackgroundJobServer();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _backgroundJobServer.Dispose();
        }

    }
}
