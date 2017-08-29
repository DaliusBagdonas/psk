using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class FreeTextAnswerViewModel
	{
		public bool Mandatory { get; set; }
		[Display(Name = "Question")]
		public string QuestionText { get; set; }
		[Required]
		[Display(Name = "Your Answer")]
		public string Answer { get; set; }
		[Required]
		public int QuestionId { get; set; }
	}
}