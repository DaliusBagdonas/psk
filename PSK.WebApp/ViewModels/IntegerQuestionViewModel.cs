using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class IntegerQuestionViewModel
	{
		[Required]
		[Display(Name = "Mandatory?")]
		public bool Mandatory { get; set; }

		[Required]
		[Display(Name = "Question Text")]
		public string QuestionText { get; set; }

		[Required]
		[Display(Name = "Upper Bound")]
		public int UpperBound { get; set; }

		[Required]
		[Display(Name = "Lower Bound")]
		public int LowerBound { get; set; }

		[Required]
		public int QuizId { get; set; }
	}
}