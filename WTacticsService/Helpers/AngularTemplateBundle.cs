using System.Text;
using System.Web.Optimization;

namespace WTacticsService.Helpers
{
    /// <summary>
    /// Allows Angular HTML-templates to be bundled in a single file.
    /// </summary>
    /// <remarks>
    /// Base on http://blog.scottlogic.com/2014/08/18/asp-angular-optimisation.html
    /// </remarks>
    public class AngularTemplateBundle : Bundle
    {
        public AngularTemplateBundle(string moduleName, string virtualPath)
            : base(virtualPath, new[] { new AngularTemplateTransform(moduleName) })
        {
        }
    }

    public class AngularTemplateTransform : IBundleTransform
    {
        private readonly string _moduleName;
        public AngularTemplateTransform(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            var strBundleResponse = new StringBuilder();
            // Javascript module for Angular that uses templateCache 
            strBundleResponse.AppendFormat(
                @"angular.module('{0}').run(['$templateCache',function(t){{",
                _moduleName);

            foreach (var file in response.Files)
            {
                // Get the partial page, remove line feeds and escape quotes
                var content = file.ApplyTransforms()
                    .Replace("\r\n", "").Replace("'", "\\'");

                var path = file.IncludedVirtualPath.Replace('\\', '/');
                if (path.StartsWith("~")) path = path.Substring(1);
                // Create insert statement with template
                strBundleResponse.AppendFormat(@"t.put('{0}','{1}');", path, content);
                strBundleResponse.AppendLine();
            }
            strBundleResponse.Append(@"}]);");

            response.Files = new BundleFile[] { };
            response.Content = strBundleResponse.ToString();
            response.ContentType = "text/javascript";
        }
    }
}