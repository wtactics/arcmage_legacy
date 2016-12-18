using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;

namespace WTacticsService
{
    public class Startup
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
            
            // Register the serializer.
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializerFactory.Value);

            app.MapSignalR();
           
        }
    }
}
