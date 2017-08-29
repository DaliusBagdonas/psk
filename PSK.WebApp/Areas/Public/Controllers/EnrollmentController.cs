using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using PSK.Model;
using PSK.Model.Abstract;
using PSK.Model.Answers;
using PSK.Model.Questions;
using PSK.NHibernate;
using PSK.WebApp.Extensions;
using PSK.WebApp.ViewModels;
using Microsoft.AspNet.Identity;
using PSK.Infrastructure.Identity;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace PSK.WebApp.Areas.Public.Controllers
{
	[AllowAnonymous]
	[RouteArea("Public")]
	[RoutePrefix("Public")]
	public class EnrollmentController : Controller
	{
		private IRepository _repository;
        private readonly ApplicationUserManager _userManager;

        public EnrollmentController(IRepository repository, ApplicationUserManager userManager)
		{
			_repository = repository;
            _userManager = userManager;
        }

		// GET: Public/Enrollment
		public ActionResult Index(int id)
		{
			var questions = _repository.GetAll<Quiz>().First(x => x.Id == id).Questions;
			Queue<AbstractQuestion> queue = new Queue<AbstractQuestion>(questions.ToList());
			System.Web.HttpContext.Current.SetQuestionQueue(queue);
			return RedirectToAction("NextQuestion");
		}

		public ActionResult NextQuestion()
		{
			AbstractQuestion question;
			try
			{
				question = System.Web.HttpContext.Current.GetNextQuestion();
			}
			catch (InvalidOperationException)
			{
				return View("Completed");
			}
			catch (NullReferenceException)
			{
				return RedirectToAction("Index", "Home");
			}
			switch (question)
			{
				case FreeTextQuestion c: return View("FreeTextAnswerForm", Mapper.Map<FreeTextQuestion, FreeTextAnswerViewModel>(c));
				case IntegerQuestion i: return View("IntegerAnswerForm", Mapper.Map<IntegerQuestion, IntegerAnswerViewModel>(i));
				case SingleChoiceQuestion s: return View("SingleChoiceAnswerForm", Mapper.Map<SingleChoiceQuestion, SingleChoiceAnswerViewModel>(s));
				case MultipleChoiceQuestion m: return View("MultiChoiceAnswerForm", Mapper.Map<MultipleChoiceQuestion, MultiChoiceAnswerViewModel>(m));
				default: throw new NotSupportedException("Unsupported question type");
			}
		}


		public ActionResult AnswerFreeText(FreeTextAnswerViewModel model)
		{
			var question = _repository.GetAll<FreeTextQuestion>().First(x => x.Id == model.QuestionId);
			var answer = new FreeTextAnswer()
			{
				Answer = model.Answer,
				AnswerTo = question,
				CorrelationId = Session.SessionID,
				Date = DateTime.Now
			};

			_repository.SaveOrUpdate(answer);
			return RedirectToAction("NextQuestion");
		}

		public ActionResult AnswerInteger(IntegerAnswerViewModel model)
		{
			var question = _repository.GetAll<IntegerQuestion>().First(x => x.Id == model.QuestionId);
			var answer = new IntegerAnswer()
			{
				Answer = model.Answer,
				AnswerTo = question,
				CorrelationId = Session.SessionID,
				Date = DateTime.Now
			};

			_repository.SaveOrUpdate(answer);
			return RedirectToAction("NextQuestion");
		}

		public ActionResult AnswerSingleChoice(SingleChoiceAnswerViewModel model)
		{
			var question = _repository.GetAll<SingleChoiceQuestion>().First(x => x.Id == model.QuestionId);
			var option = question.Options.First(x => x.Id == model.AnswerId);
			var answer = new SingleChoiceAnswer()
			{
				Answer = option,
				AnswerTo = question,
				CorrelationId = Session.SessionID,
				Date = DateTime.Now
			};

			_repository.SaveOrUpdate(answer);
			return RedirectToAction("NextQuestion");
		}

		public ActionResult AnswerMultiChoice(MultiChoiceAnswerViewModel model, string answers)
		{

			var question = _repository.GetAll<MultipleChoiceQuestion>().First(x => x.Id == model.QuestionId);
			var selectedAnswers = answers.Split(new string[] { "," }, StringSplitOptions.None);
			var map = new List<ChoiceOption>();

			foreach (var s in selectedAnswers)
			{
				var option = new ChoiceOption() { Text = s };
				map.Add(option);
			}

			var answer = new MultipleChoiceAnswer()
			{
				Answer = map,
				AnswerTo = question,
				CorrelationId = Session.SessionID,
				Date = DateTime.Now
			};

			_repository.SaveOrUpdate(answer);
			return RedirectToAction("NextQuestion");
		}

        [Route("SaveAndSend/{id}/{email}")]
        public async Task<ActionResult> SaveAndSend(int id, string email)
        {
            var quizId = _repository.GetAll<Quiz>().Where(x => x.Questions.Any(z => z.Id == id)).First().Id;
            var fullUrl = Url.Action("Continue", "Enrollment", new { quizId = quizId, correlationId = Session.SessionID }, Request.Url.Scheme);

            IdentityMessage message = new IdentityMessage() { Subject = "Your Quiz", Body = fullUrl, Destination = email.Replace("%2E", ".")};

            await _userManager.EmailService.SendAsync(message);
            return View("Completed");
            //TODO: callback ar nusisiuntė.
        }

        [Route("Continue/{quizId}/{correlationId}")]
        public ActionResult Continue(int quizId, string correlationId)
        {
            List<int> answeredQuestionsIds = new List<int>();
            answeredQuestionsIds.AddRange(_repository.GetAll<FreeTextAnswer>().Where(m => m.AnswerTo.Quiz.Id == quizId && m.CorrelationId == correlationId).Select(m => m.AnswerTo.Id).ToList());
            answeredQuestionsIds.AddRange(_repository.GetAll<IntegerAnswer>().Where(m => m.AnswerTo.Quiz.Id == quizId && m.CorrelationId == correlationId).Select(m => m.AnswerTo.Id).ToList());
            answeredQuestionsIds.AddRange(_repository.GetAll<SingleChoiceAnswer>().Where(m => m.AnswerTo.Quiz.Id == quizId && m.CorrelationId == correlationId).Select(m => m.AnswerTo.Id).ToList());
            answeredQuestionsIds.AddRange(_repository.GetAll<MultipleChoiceAnswer>().Where(m => m.AnswerTo.Quiz.Id == quizId && m.CorrelationId == correlationId).Select(m => m.AnswerTo.Id).ToList());
            answeredQuestionsIds.Distinct();

            var questions = _repository.GetAll<Quiz>().First(x => x.Id == quizId).Questions.ToList();
            var notAnswered = questions.Where(p => !answeredQuestionsIds.Any(p2 => p2 == p.Id)).ToList();
            
            Queue<AbstractQuestion> queue = new Queue<AbstractQuestion>(notAnswered);
            System.Web.HttpContext.Current.SetQuestionQueue(queue);
            return RedirectToAction("NextQuestion");
        }

        public ActionResult QuizList()
        {
            return View();
        }


        [HttpGet]
        public ActionResult GetQuizsForAnswering()
        {
            var data = _repository.GetAll<Quiz>().ToList();
            var quizes = data.Select(x => Mapper.Map<QuizViewModel>(x)).ToList();
            return PartialView("_QuizList", quizes);
        }
    }
}