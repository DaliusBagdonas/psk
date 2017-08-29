using System.Collections.Generic;
using System.Web;
using Microsoft.Ajax.Utilities;
using PSK.Model.Abstract;

namespace PSK.WebApp.Extensions
{
	public static class HttpContextExtensions
	{
		public static Queue<AbstractQuestion> GetQuestionQueue(this HttpContext context)
		{
			return (Queue<AbstractQuestion>)context.Session["__Questions"];
		}

		public static AbstractQuestion GetNextQuestion(this HttpContext context)
		{
			return ((Queue<AbstractQuestion>)context.Session["__Questions"]).Dequeue();
		}

		public static void SetQuestionQueue(this HttpContext context, Queue<AbstractQuestion> questions)
		{
			context.Session["__Questions"] = questions;
		}
	}
}