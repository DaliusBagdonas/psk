using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.Models
{
	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}