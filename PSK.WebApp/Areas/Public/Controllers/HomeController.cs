using System.Linq;
using System.Web.Mvc;
using PSK.Model;
using PSK.NHibernate;

namespace PSK.WebApp.Areas.Public.Controllers
{
	public class HomeController : Controller
	{
		private IRepository _repository;


		public HomeController(IRepository repository)
		{
			_repository = repository;
		}

		[AllowAnonymous]
		public ActionResult Index()
		{
			ViewBag.Message = "Hi!";

			return View();
		}
	}
}