using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PSK.WebApp
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AutoMapperConfiguration.Configure();
			//ViewEngines.Engines.Clear();
			//ViewEngines.Engines.Add(new AreaAwareViewEngine());
		}

		void Session_Start(object sender, EventArgs e)
		{
			// ASP.NET keeps assigning new session ids until you place something into the Session variable.
			System.Web.HttpContext.Current.Session.Add("__STAPH_PLS", string.Empty);
		}
	}
}
