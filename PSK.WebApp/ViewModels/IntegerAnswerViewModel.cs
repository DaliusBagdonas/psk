using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class IntegerAnswerViewModel
	{
		public bool Mandatory { get; set; }
		[Display(Name = "Question")]
		public string QuestionText { get; set; }

		public int UpperBound { get; set; }
		public int LowerBound { get; set; }

		[Required]
		[Display(Name = "Your Answer")]
		public int Answer { get; set; }

		[Required]
		public int QuestionId { get; set; }
	}
}