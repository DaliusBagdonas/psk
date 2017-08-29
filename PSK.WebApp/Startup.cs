using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using NHibernate;
using Owin;

[assembly: OwinStartupAttribute(typeof(PSK.WebApp.Startup))]
namespace PSK.WebApp
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			AutofacConfig.ConfigureContainer(app);
			ConfigureAuth(app);
		}


	}
}
