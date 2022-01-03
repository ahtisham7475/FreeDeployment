using Cobisi.EmailVerify;
using Cobisi.Net.Proxy;
using log4net.Config;
using swift.api.ui;
using System;
using System.Web.Routing;

namespace swift.api
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();       
            EmailVerifier.RuntimeLicenseKey = "PbsTgqe8Qwd/7gymzRp8lcy/aK+l2NWDsmtZVuT0TS1pU6uF6KeQtXHtS8vHASr04Kfbfg==";
            ProxyClient.RuntimeLicenseKey = "FiIw7Mw1M+PSAjjKJ+BNUUmIQdOTJZpcQ+GEMhJpAeO0Zj5sg4M80Ipe4y3KGghq2nX1Gg==";
            RegisterRoutes(RouteTable.Routes);
        }

        void RegisterRoutes(RouteCollection routes)
        {
            // Register a route for email checker
            routes.Add("Default", new Route("CheckEmail/{Account}/{Email}", new Start()));
        }
    }
}