using NHibernate;
using NHibernate.AspNet.Identity;

namespace PSK.Infrastructure.Identity
{
	public class ApplicationRoleStore : RoleStore<IdentityRole>
	{
		public ApplicationRoleStore(ISession context) : base(context)
		{
		}
	}


}