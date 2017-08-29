using NHibernate;
using NHibernate.AspNet.Identity;
using PSK.Model.Identity;

namespace PSK.Infrastructure.Identity
{
	public class ApplicationUserStore : UserStore<ApplicationUser>
	{
		public ApplicationUserStore(ISession context)
			: base(context)
		{
		}
	}
}