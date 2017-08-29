using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using NHibernate;
using NHibernate.AspNet.Identity;
using Owin;
using PSK.Infrastructure.Identity;
using PSK.Model;
using PSK.Model.Identity;
using PSK.NHibernate;

namespace PSK.WebApp
{
	public class AutofacConfig
	{
		public static void ConfigureContainer(IAppBuilder app)
		{
			var builder = new ContainerBuilder();

			builder.Register(c => SessionFactoryProvider.Get().OpenSession());
			// REGISTER DEPENDENCIES
			builder.Register(c =>
					new Repository(
						new Lazy<ISession>(
							() => SessionFactoryProvider.Get().OpenSession())))
				.As<IRepository>();


			// Register dependencies in filter attributes
			builder.RegisterFilterProvider();

			// Register dependencies in custom views
			builder.RegisterSource(new ViewRegistrationSource());
			builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser>>().InstancePerRequest();
			builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
			builder.RegisterType<ApplicationRoleStore>().As<IRoleStore<IdentityRole>>().InstancePerRequest();
			builder.RegisterType<ApplicationRoleManager>().AsSelf().InstancePerRequest();
			builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerRequest();
			builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication)
				.InstancePerRequest();
			builder.Register<IDataProtectionProvider>(c => app.GetDataProtectionProvider()).InstancePerRequest();

			// REGISTER CONTROLLERS SO DEPENDENCIES ARE CONSTRUCTOR INJECTED
			builder.RegisterControllers(typeof(MvcApplication).Assembly);

			// BUILD THE CONTAINER
			var container = builder.Build();

			// REPLACE THE MVC DEPENDENCY RESOLVER WITH AUTOFAC
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

			// REGISTER WITH OWIN
			app.UseAutofacMiddleware(container);
			app.UseAutofacMvc();
		}
	}
}