using System.Web.Http;
using System.Web.Routing;
using Umbraco.Core;

namespace PhoenixConverters.Events
{
    public class Events
    {
        public class ApplicationStartUp : IApplicationEventHandler
        {

            public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
            {

            }

            public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
            {

            }

            public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
            {
                GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
                MapWebApiRoutes();
            }

            private void MapWebApiRoutes()
            {
                RouteTable.Routes.MapHttpRoute("PhoenixApi", "umbraco/backoffice/PhoenixApi/{action}/{alias}",
                        new
                        {
                            controller = "PhoenixApi"
                        }
                );
                RouteTable.Routes.MapHttpRoute("PhoenixApiTest", "umbraco/backoffice/PhoenixApi/Perform/{action}/{alias}/{dataTypeId}",
                        new
                        {
                            controller = "PhoenixApi"
                        }
                );
                RouteTable.Routes.MapHttpRoute("PhoenixApiDataTypeAlias", "umbraco/backoffice/PhoenixApi/DataType/{action}/{dataTypeAlias}",
                        new
                        {
                            controller = "PhoenixApi"
                        }
                );
            }
        }
    }
}
