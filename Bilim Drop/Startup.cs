using Owin;
using System.Web.Http;

namespace Bilim_Drop
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "bilimdrop/{controller}/{id}",
                defaults: new { controller = "index", id = RouteParameter.Optional }
            );
            app.UseWebApi(config);
        }
    }
}
