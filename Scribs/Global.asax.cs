using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace Scribs
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest() {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS") {
                Response.End();
                Response.Flush();
            }
        }
    }
}
