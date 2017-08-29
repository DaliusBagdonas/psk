using PSK.Model.Questions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class ChoiceQuestionViewModel
    {
		[Required]
		[Display(Name = "Mandatory?")]
		public bool Mandatory { get; set; }

		[Required]
		[Display(Name = "Question Text")]
		public string QuestionText { get; set; }

		[Required]
        [Display(Name = "Possible Answers")]
        public List<ChoiceOption> Answers { get; set; }

        [Required]
		public int QuizId { get; set; }
	}
}