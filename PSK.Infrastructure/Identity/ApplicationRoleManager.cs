using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity;

namespace PSK.Infrastructure.Identity
{
	public class ApplicationRoleManager : RoleManager<IdentityRole>
	{
		public ApplicationRoleManager(IRoleStore<IdentityRole> store) : base(store)
		{
		}
	}
}