using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
