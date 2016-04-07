using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WTacticsService
{
    public static class GeneralConfig
    {
        public static void Config(HttpConfiguration config)
        {
         
            // We disable XML in the REST API because:
            // * it has poor support for dynamic and polymorphic objects. 
            // * We don't want to wory about XML namespaces
            // * Our clients are browsers that expect JSON anyway.
            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            // We configure the JSON formatter so it uses camelCasing (as in Javascript) instead of PascalCasing (as in .NET)
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;  // Overhead is negligible but much easier to debug
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;

            // Set default JSON settings for non-ASP code (mainly this is used when serializing JSON for the database)
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
        }
    }
}