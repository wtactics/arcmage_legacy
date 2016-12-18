using System;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.Http.Cors;

namespace WTacticsGameService
{
    public class GameServiceStartup
    {
       
        private static readonly Lazy<JsonSerializer> JsonSerializerFactory = new Lazy<JsonSerializer>(GetJsonSerializer);
        private static JsonSerializer GetJsonSerializer()
        {
            var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new SignalRContractResolver() };
            serializer.Converters.Add(new StringEnumConverter() { CamelCaseText = true });

            return serializer;
        }


        public void Configuration(IAppBuilder app)
        {

            HttpConfiguration config = new HttpConfiguration();
            // We disable XML in the REST API because:
            // * it has poor support for dynamic and polymorphic objects. 
            // * We don't want to wory about XML namespaces
            // * Our clients are browsers that expect JSON anyway.
            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            // We configure the JSON formatter so it uses camelCasing (as in Javascript) instead of PascalCasing (as in .NET)
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.None;  // Overhead is negligible but much easier to debug
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter() { CamelCaseText = true });

            // Set default JSON settings for non-ASP code (mainly this is used when serializing JSON for the database)
            JsonConvert.DefaultSettings = () =>
            {
                var defaultsettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                };
                defaultsettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter() { CamelCaseText = true });
                return defaultsettings;
            };



            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            app.UseWebApi(config);
           
            // Register the serializer.
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializerFactory.Value);
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();

        }

    }
}
