using System;
using System.ComponentModel.DataAnnotations;

namespace PSK.WebApp.ViewModels
{
	public class QuestionListItemViewModel
	{

		public int Id { get; set; }
		[Display(Name = "Mandatory")]
		public bool Mandatory { get; set; }

		[Display(Name = "Type")]
		public string Type { get; set; }

		[Display(Name = "Question Text")]
		public string QuestionText { get; set; }
	}
}