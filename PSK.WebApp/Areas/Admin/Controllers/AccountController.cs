using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NHibernate.AspNet.Identity;
using NHibernate.Linq;
using PSK.Infrastructure.Identity;
using PSK.Model.Identity;
using PSK.WebApp.Models;
using PSK.WebApp.ViewModels;

namespace PSK.WebApp.Areas.Admin.Controllers
{
	[Authorize(Roles = "Admin")]
	[RouteArea("Admin")]
	[RoutePrefix("Account")]
	public class AccountController : Controller
	{
		private ApplicationRoleManager _roleManager;
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,
			ApplicationRoleManager roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[Route("")]
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult GetUsers()
		{

			var users = _userManager.Users.Select(x => Mapper.Map<ApplicationUser, ApplicationUserListItemViewModel>(x)).ToList();
			return PartialView("_UserGrid", users);

		}


		//
		// GET: /Account/Register
		[HttpGet]
		[Route("Register")]
		public ActionResult Register()
		{
			var list = _roleManager.Roles.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name });
			ViewBag.Role = list.ToList();
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Register")]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await _userManager.AddToRoleAsync(user.Id, model.Role);
					//await _signInManager.SignInAsync(user, false, false);
					return RedirectToAction("Index", "Account");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}


		//
		// GET: /Account/CreateRole
		[HttpGet]
		[Route("CreateRole")]
		public ActionResult CreateRole()
		{
			return View();
		}

		//
		// POST: /Account/CreateRole
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("CreateRole")]
		public async Task<ActionResult> CreateRole(IdentityRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Account");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError("", error);
		}

	}
}