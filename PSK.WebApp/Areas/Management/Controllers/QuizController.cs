using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using PSK.Infrastructure.Identity;
using PSK.Model;
using PSK.Model.Abstract;
using PSK.Model.Questions;
using PSK.NHibernate;
using PSK.WebApp.ViewModels;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.OleDb;
using System;
using PSK.Model.Answers;
using System.Threading;
using PSK.Model.Identity;
using System.IO;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.Net.Http;
using System.Diagnostics;

namespace PSK.WebApp.Areas.Management.Controllers
{
	[Authorize(Roles = "Admin,Manager")]
	[RouteArea("Management")]
	[RoutePrefix("Quiz")]
	public class QuizController : Controller
	{
		private readonly IRepository _repository;
		private readonly ApplicationUserManager _userManager;

		public QuizController(IRepository repository, ApplicationUserManager userManager)
		{
			_repository = repository;
			_userManager = userManager;
		}

		[Route("")]
		public ActionResult Index()
		{
            return View();
		}

		[Route("Create")]
		public ActionResult CreateQuiz()
		{
			return View();
		}

		[Route("Details")]
		public ActionResult Details(int id)
		{
			var model = _repository.GetAll<Quiz>().First(x => x.Id == id);
			var vm = Mapper.Map<QuizViewModel>(model);
			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Create")]
		public async Task<ActionResult> CreateQuiz(QuizViewModel model)
		{
			var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
			var quiz = new Quiz(model.Name, user);
			_repository.SaveOrUpdate(quiz);

            return RedirectToAction("Details", new { quiz.Id });
		}

		[HttpGet]
		public ActionResult GetQuizs()
		{
			var data = _repository.GetAll<Quiz>().ToList();
			var something = data.Select(x => Mapper.Map<QuizViewModel>(x)).ToList();
			return PartialView("_QuizGrid", something);
		}

		[HttpGet]
		public ActionResult GetQuestions(int id)
		{
			var items = _repository.GetAll<Quiz>().First(x => x.Id == id).Questions;
			var data = items.Select(x => Mapper.Map<AbstractQuestion, QuestionListItemViewModel>(x));
			return PartialView("_QuestionGrid", data.ToList());
		}

		[HttpGet]
		public ActionResult FreeTextQuestionForm(int id)
		{
			var model = new FreeTextQuestionViewModel { QuizId = id };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateFreeTextQuestion(FreeTextQuestionViewModel model)
		{
			var parent = _repository.GetAll<Quiz>().First(x => x.Id == model.QuizId);
			var entity = new FreeTextQuestion { Mandatory = model.Mandatory, QuestionText = model.QuestionText, Quiz = parent };
			_repository.SaveOrUpdate(entity);
			return RedirectToAction("Details", new { id = model.QuizId });
		}

		[HttpGet]
		public ActionResult IntegerQuestionForm(int id)
		{
			var model = new IntegerQuestionViewModel { QuizId = id };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateIntegerQuestion(IntegerQuestionViewModel model)
		{
			var parent = _repository.GetAll<Quiz>().First(x => x.Id == model.QuizId);
			var entity = new IntegerQuestion
			{
				Mandatory = model.Mandatory,
				QuestionText = model.QuestionText,
				Quiz = parent,
				LowerBound = model.LowerBound,
				UpperBound = model.UpperBound
			};
			_repository.SaveOrUpdate(entity);
			return RedirectToAction("Details", new { id = model.QuizId });
		}

        [HttpGet]
        public ActionResult SingleChoiceQuestionForm(int id)
        {
            var model = new ChoiceQuestionViewModel { QuizId = id };
            model.Answers = new List<ChoiceOption> { new ChoiceOption() };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSingleChoiceQuestion(ChoiceQuestionViewModel model)
        {
            var parent = _repository.GetAll<Quiz>().First(x => x.Id == model.QuizId);
            var entity = new SingleChoiceQuestion
            {
                Mandatory = model.Mandatory,
                QuestionText = model.QuestionText,
                Quiz = parent,
                Options = model.Answers
            };
            _repository.SaveOrUpdate(entity);

            return RedirectToAction("Details", new { id = model.QuizId });
        }

        [HttpGet]
        public ActionResult MultiChoiceQuestionForm(int id)
        {
            var model = new ChoiceQuestionViewModel { QuizId = id };
            model.Answers = new List<ChoiceOption> { new ChoiceOption() };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultiChoiceQuestion(ChoiceQuestionViewModel model)
        {
            var parent = _repository.GetAll<Quiz>().First(x => x.Id == model.QuizId);
            var entity = new MultipleChoiceQuestion
            {
                Mandatory = model.Mandatory,
                QuestionText = model.QuestionText,
                Quiz = parent,
                Options = model.Answers
            };
            _repository.SaveOrUpdate(entity);

            return RedirectToAction("Details", new { id = model.QuizId });
        }

        [HttpGet]
        public ActionResult ImportQuiz()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportQuiz(HttpPostedFileBase file, QuizViewModel model)
        {
            var user = _userManager.FindById(User.Identity.GetUserId());

            if (file.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName);

                string fileLocation = Server.MapPath("~/Content/") + file.FileName;
                if (System.IO.File.Exists(fileLocation))
                {
                    try
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    catch (Exception)
                    {
                        TempData["message"] = "This quiz was already imported!";
                        return RedirectToAction("ImportQuiz");
                    }
                }

                ThreadStart callback = (() => ImportFromExcel(file, user, Session.SessionID, model.Name));

                callback += () => {
                    //TODO: kažkas kai baigia importuot???
                };

                Thread thread = new Thread(callback) { IsBackground = true};
                thread.Start();
            }
            return View();
        }

        private bool ImportFromExcel(HttpPostedFileBase file, ApplicationUser user, string sessionId, string quizName)
        {
            DataSet ds = new DataSet();

            string fileExtension = Path.GetExtension(file.FileName);
            string fileLocation = Server.MapPath("~/Content/") + file.FileName;

            file.SaveAs(fileLocation); 
            string excelConnectionString = string.Empty;
            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

            if (fileExtension == ".xls")
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
            }

            else if (fileExtension == ".xlsx")
            {
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
            }

            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
            excelConnection.Open();
            DataTable dt = new DataTable();

            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt == null) return false;

            String[] excelSheets = new String[dt.Rows.Count];
            int t = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheets[t] = row["TABLE_NAME"].ToString();
                t++;
            }
            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

            foreach (var e in excelSheets.Where(x => x.ToString() == "Survey$" || x.ToString() == "Answer$"))
            {
                string query = string.Format("Select * from [{0}]", e);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                dataAdapter.Fill(ds, e.ToString());
            }

            var questionNumber = 0;
            var question = "";
            var type = "";
            var mandatory = "";

            var quiz = new Quiz(quizName, user);
            _repository.SaveOrUpdate(quiz);

            var surveyTable = ds.Tables[0].ToString() == "Survey$" ? ds.Tables[0] : ds.Tables[1];
            var answerTable = ds.Tables[0].ToString() == "Answer$" ? ds.Tables[0] : ds.Tables[1];
            // TODO : patikrinti ar rado lenteles
            for (int i = 0; i < surveyTable.Rows.Count; i++)
            {
                if (surveyTable.Rows[i].ItemArray[0].ToString() == "") break;
                var row = surveyTable.Rows[i].ItemArray.Where(c => c.ToString() != "").ToArray();
                if (row.Count() < 2) continue;

                var possibleAnswers = new List<ChoiceOption>();

                //TODO reiktų patikrinti ir ar čia reikšmės egzistuoja. xD
                mandatory = row[1].ToString();
                question = row[2].ToString();
                type = row[3].ToString();
                questionNumber = Int32.Parse(row[0].ToString());

                var answers = answerTable.Select("$questionNumber = " + questionNumber);

                if (type.ToUpper() == "TEXT")
                {
                    var entity = new FreeTextQuestion { QuestionText = question, Quiz = quiz, Mandatory = mandatory == "YES" ? true : false };
                    _repository.SaveOrUpdate(entity);

                    var questionId = entity.Id;

                    for (int a = 0; a < answers.Count(); a++)
                    {
                        var answerRow = answers[a].ItemArray.Where(x => x.ToString() != "").ToList();
                        var answer = new FreeTextAnswer()
                        {
                            Answer = answerRow[2].ToString(), // manom, kad jei jau yra įvesta atsakymo eilutė tai bus ir variantas įvestas.
                            AnswerTo = entity,
                            CorrelationId = sessionId,
                            Date = DateTime.Now
                        };
                        _repository.SaveOrUpdate(answer);
                    }

                    continue;
                }

                if (type.ToUpper() == "SCALE")
                {
                    var entity = new IntegerQuestion
                    {
                        QuestionText = question,
                        Quiz = quiz,
                        LowerBound = Int32.Parse(row[4].ToString()),
                        UpperBound = Int32.Parse(row[5].ToString()),
                        Mandatory = mandatory == "YES" ? true : false
                    };
                    _repository.SaveOrUpdate(entity);

                    for (int a = 0; a < answers.Count(); a++)
                    {
                        var answerRow = answers[a].ItemArray.Where(x => x.ToString() != "").ToList();
                        var answer = new IntegerAnswer()
                        {
                            Answer = Int32.Parse(answerRow[2].ToString()),
                            AnswerTo = entity,
                            CorrelationId = sessionId,
                            Date = DateTime.Now
                        };
                        _repository.SaveOrUpdate(answer);
                    }

                    continue;
                }

                for (int j = 4; j < row.Count(); j++)
                {
                    var option = new ChoiceOption() { Text = row[j].ToString() };
                    possibleAnswers.Add(option);
                }

                if (type.ToUpper() == "MULTIPLECHOICE")
                {
                    var entity = new SingleChoiceQuestion
                    {
                        QuestionText = question,
                        Options = possibleAnswers,
                        Quiz = quiz,
                        Mandatory = mandatory == "YES" ? true : false
                    };
                    _repository.SaveOrUpdate(entity);

                    for (var a = 0; a < answers.Count(); a++)
                    {
                        var answerRow = answers[a].ItemArray.Where(x => x.ToString() != "").ToList();
                        var selectedAnswer = Int32.Parse(answerRow[2].ToString());

                        var answer = new SingleChoiceAnswer()
                        {
                            Answer = possibleAnswers[selectedAnswer - 1],
                            AnswerTo = entity,
                            CorrelationId = sessionId,
                            Date = DateTime.Now
                        };
                        _repository.SaveOrUpdate(answer);
                    }

                    continue;
                }

                if (type.ToUpper() == "CHECKBOX")
                {
                    var entity = new MultipleChoiceQuestion
                    {
                        QuestionText = question,
                        Options = possibleAnswers,
                        Quiz = quiz,
                        Mandatory = mandatory == "YES" ? true : false
                    };
                    _repository.SaveOrUpdate(entity);

                    for (var a = 0; a < answers.Count(); a++)
                    {
                        var answerRow = answers[a].ItemArray.Where(x => x.ToString() != "").ToList();
                        var selectedAnswers = new List<ChoiceOption>();

                        for (int ar = 2; ar < answerRow.Count(); ar++)
                        {
                            var indexId = Int32.Parse(answerRow[ar].ToString()) - 1;
                            selectedAnswers.Add(possibleAnswers[indexId]);
                        }

                        var answer = new MultipleChoiceAnswer()
                        {
                            Answer = selectedAnswers,
                            AnswerTo = entity,
                            CorrelationId = sessionId,
                            Date = DateTime.Now
                        };

                        _repository.SaveOrUpdate(answer);
                    }
                }
            }

            return true;
        }

        [Route("ExportQuiz")]
        public ActionResult ExportQuiz(int id)
        {
            ThreadStart callback = (() => Export(id));

            Thread thread = new Thread(callback) { IsBackground = true };
            thread.Start();

            return View("Index");
        }

        private void Export(int id)
        {
            var freeTextQuestions = _repository.GetAll<FreeTextQuestion>().Where(x => x.Quiz.Id == id).ToList();
            var integerQuestions = _repository.GetAll<IntegerQuestion>().Where(x => x.Quiz.Id == id).ToList();
            var singleChoiceQuestions = _repository.GetAll<SingleChoiceQuestion>().Where(x => x.Quiz.Id == id).ToList();
            var multiChoiceQuestions = _repository.GetAll<MultipleChoiceQuestion>().Where(x => x.Quiz.Id == id).ToList();

            DataColumn[] q = new DataColumn[] {
                new DataColumn("$questionNumber", typeof(string)),
                new DataColumn("$question", typeof(string)),
                new DataColumn("$questionType", typeof(string)),
                new DataColumn("$optionsList", typeof(string))
            };

            DataColumn[] a = new DataColumn[] {
                new DataColumn("$answerID", typeof(string)),
                new DataColumn("$questionNumber", typeof(string)),
                new DataColumn("$answer", typeof(string))
            };

            DataSet ds = new DataSet();
            DataTable questionsTable = new DataTable("Survey");
            questionsTable.Columns.AddRange(q);

            DataTable answersTable = new DataTable("Answer");
            answersTable.Columns.AddRange(a);

            if (integerQuestions.Count() != 0) questionsTable.Columns.Add();

            for (int i = 0; i < freeTextQuestions.Count(); i++)
            {
                questionsTable.Rows.Add(freeTextQuestions[i].Id, freeTextQuestions[i].QuestionText, "TEXT");
                var freeTextAnswers = _repository.GetAll<FreeTextAnswer>().Where(x => x.AnswerTo.Id == freeTextQuestions[i].Id).ToList();

                for (int j = 0; j < freeTextAnswers.Count(); j++)
                    answersTable.Rows.Add(freeTextAnswers[j].Id, freeTextAnswers[j].AnswerTo.Id, freeTextAnswers[j].Answer);
            }

            for (int i = 0; i < integerQuestions.Count(); i++)
            {
                questionsTable.Rows.Add(integerQuestions[i].Id, integerQuestions[i].QuestionText, "SCALE", integerQuestions[i].LowerBound, integerQuestions[i].UpperBound);
                var integerAnswers = _repository.GetAll<IntegerAnswer>().Where(x => x.AnswerTo.Id == integerQuestions[i].Id).ToList();

                for (int j = 0; j < integerAnswers.Count(); j++)
                    answersTable.Rows.Add(integerAnswers[j].Id, integerAnswers[j].AnswerTo.Id, integerAnswers[j].Answer);
            }

            for (int i = 0; i < singleChoiceQuestions.Count(); i++)
            {
                DataRow row = questionsTable.NewRow();
                var singleChoiceAnswers = _repository.GetAll<SingleChoiceAnswer>().Where(x => x.AnswerTo.Id == singleChoiceQuestions[i].Id).ToList();
                var options = new List<string>();

                row[0] = singleChoiceQuestions[i].Id;
                row[1] = singleChoiceQuestions[i].QuestionText;
                row[2] = "MULTIPLECHOICE";

                for (int j = 0; j < singleChoiceQuestions[i].Options.Count(); j++)
                {
                    if (j > 0 && questionsTable.Columns.Count <= j + 3) questionsTable.Columns.Add();
                    row[3 + j] = singleChoiceQuestions[i].Options[j].Text;
                    options.Add(singleChoiceQuestions[i].Options[j].Text);
                }

                questionsTable.Rows.Add(row);

                for (int j = 0; j < singleChoiceAnswers.Count(); j++)
                {
                    var selectedOption = options.IndexOf(singleChoiceAnswers[j].Answer.Text);
                    answersTable.Rows.Add(singleChoiceAnswers[j].Id, singleChoiceAnswers[j].AnswerTo.Id, ++selectedOption);
                }
            }

            for (int i = 0; i < multiChoiceQuestions.Count(); i++)
            {
                DataRow row = questionsTable.NewRow();
                var options = new List<string>();
                var selected = new List<string>();

                row[0] = multiChoiceQuestions[i].Id;
                row[1] = multiChoiceQuestions[i].QuestionText;
                row[2] = "CHECKBOX";

                for (int j = 0; j < multiChoiceQuestions[i].Options.Count(); j++)
                {
                    if (j > 0 && questionsTable.Columns.Count <= j + 3) questionsTable.Columns.Add();
                    row[3 + j] = multiChoiceQuestions[i].Options[j].Text;
                    options.Add(multiChoiceQuestions[i].Options[j].Text);
                }

                questionsTable.Rows.Add(row);
                var multiChoiceAnswers = _repository.GetAll<MultipleChoiceAnswer>().Where(x => x.AnswerTo.Id == multiChoiceQuestions[i].Id).ToList();

                for (int answer = 0; answer < multiChoiceAnswers.Count(); answer++)
                {
                    DataRow answerRow = answersTable.NewRow();
                    answerRow[0] = multiChoiceAnswers[answer].Id;
                    answerRow[1] = multiChoiceAnswers[answer].AnswerTo.Id;

                    for (int j = 0; j < multiChoiceAnswers[answer].Answer.Count(); j++)
                    {
                        if (j > 0 && answersTable.Columns.Count <= j + 2) answersTable.Columns.Add();
                        answerRow[2 + j] = options.IndexOf(multiChoiceAnswers[answer].Answer[j].Text) + 1;
                    }

                    answersTable.Rows.Add(answerRow);
                }
            }

            ds.Tables.Add(questionsTable);
            ds.Tables.Add(answersTable);

            XLWorkbook wb = new XLWorkbook();
            using (wb)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    wb.Worksheets.Add(dt);
                }
            }

            var quizName = _repository.GetAll<Quiz>().First(x => x.Id == id).Name;
            string path = Server.MapPath("~/Content/Exported/" + quizName + ".xlsx");
            wb.SaveAs(path);
            Process.Start(path);
        }

        [Route("Report")]
        public ActionResult Report(int id)
        {
            List<FreeText> freeText = new List<FreeText>();
            List<Integer> integer = new List<Integer>();
            List<SingleChoice> singleChoice = new List<SingleChoice>();
            List<MultiChoice> multiChoice = new List<MultiChoice>();
            var questions = _repository.GetAll<Quiz>().First(x => x.Id == id).Questions;

            foreach (var q in questions)
            {
                var a = q.GetType();
                switch (q.GetType().Name.ToString())
                {
                    case "FreeTextQuestion" :
                        List<FreeTextAnswer> answers = _repository.GetAll<FreeTextAnswer>().Where(x => x.AnswerTo.Id == q.Id).ToList();
                        if (answers.Count() > 0)
                        {
                            FreeText item = new FreeText()
                            {
                                FreeTextAnswers = answers,
                                Question = q
                            };
                            freeText.Add(item);
                        }
                        break;
                    case "IntegerQuestion":
                        List<IntegerAnswer> integerAnswers = _repository.GetAll<IntegerAnswer>().Where(x => x.AnswerTo.Id == q.Id).ToList();

                        if (integerAnswers.Count() > 0)
                        {
                            double average = integerAnswers.Average(x => x.Answer);
                            int lowest = integerAnswers.Min(x => x.Answer);
                            int highest = integerAnswers.Max(x => x.Answer);

                            Integer itemInteger = new Integer()
                            {
                                IntegertAnswers = integerAnswers,
                                Question = q,
                                LowestScore = lowest,
                                HighestScore = highest,
                                AverageScore = average
                            };
                            integer.Add(itemInteger);
                        }
                        break;
                    case "SingleChoiceQuestion":
                        List<SingleChoiceAnswer> singleChoiceAnswers = _repository.GetAll<SingleChoiceAnswer>().Where(x => x.AnswerTo.Id == q.Id).ToList();

                        if (singleChoiceAnswers.Count() > 0)
                        {
                            var mostCommon = singleChoiceAnswers.GroupBy(x => x.Answer.Text).OrderByDescending(gp => gp.Count()).Take(2).Select(g => g.Key).ToList(); //TODO:o kai yra keli variantai daugiausiai atsakytų??
                            SingleChoice itemSingle = new SingleChoice()
                            {
                                Question = q,
                                SingleChoiceAnswers = singleChoiceAnswers,
                                MostCommonAnswers = mostCommon
                            };

                            singleChoice.Add(itemSingle);
                        }
                        break;
                    case "MultipleChoiceQuestion":
                        List<MultipleChoiceAnswer> multiChoiceAnswers = _repository.GetAll<MultipleChoiceAnswer>().Where(x => x.AnswerTo.Id == q.Id).ToList();
                        List<string> selectedOptions = new List<string>();

                        if (multiChoiceAnswers.Count() > 0)
                        {
                            foreach (var multi in multiChoiceAnswers)
                                foreach (var m in multi.Answer) selectedOptions.Add(m.Text);

                            var mostCommonMultiple = selectedOptions.GroupBy(x => x).OrderByDescending(gp => gp.Count()).Take(2).Select(g => g.Key).ToList(); //TODO:o kai yra keli variantai daugiausiai atsakytų??

                            MultiChoice itemMulti = new MultiChoice()
                            {
                                Question = q,
                                MultiChoiceAnswers = multiChoiceAnswers,
                                MostCommonAnswers = mostCommonMultiple
                            };

                            multiChoice.Add(itemMulti);
                        }

                        break;
                    default:
                        break;
                }
            }
            var model = new ReportViewModel(freeText, integer, singleChoice, multiChoice);

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}