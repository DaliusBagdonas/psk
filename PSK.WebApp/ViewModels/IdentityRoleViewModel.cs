using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class IdentityRoleViewModel
	{
		[Required]
		[Display(Name = "Role Name")]
		public string Name { get; set; }
	}
}