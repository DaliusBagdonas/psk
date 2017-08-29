using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PSK.WebApp.ViewModels
{
	public class SingleChoiceAnswerViewModel
	{
		public bool Mandatory { get; set; }
		[Display(Name = "Question")]
		public string QuestionText { get; set; }

		[Required]
		[Display(Name = "Your Answer")]
		public int AnswerId { get; set; }

		[Required]
		public int QuestionId { get; set; }

		public List<SelectListItem> Options { get; set; }

	}
}