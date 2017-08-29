using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class FreeTextQuestionViewModel
	{
		[Required]
		[Display(Name = "Mandatory?")]
		public bool Mandatory { get; set; }

		[Required]
		[Display(Name = "Question Text")]
		public string QuestionText { get; set; }

		[Required]
		public int QuizId { get; set; }
	}
}