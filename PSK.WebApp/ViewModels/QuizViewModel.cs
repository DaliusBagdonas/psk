using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class QuizViewModel
	{
		public string Id { get; set; }

		[Required]
		[Display(Name = "Quiz Name")]
		public string Name { get; set; }

		[Display(Name = "Created By")]
		public string CreatedBy { get; set; }
	}
}