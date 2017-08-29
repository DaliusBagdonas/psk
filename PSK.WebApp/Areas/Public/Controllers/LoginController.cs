using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NHibernate.AspNet.Identity;
using PSK.Infrastructure.Identity;
using PSK.Model.Identity;
using PSK.WebApp.Models;

namespace PSK.WebApp.Areas.Public.Controllers
{
	[AllowAnonymous]
	public class LoginController : Controller
	{
		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;
		private ApplicationRoleManager _roleManager;

		public LoginController(ApplicationSignInManager signInManager, ApplicationUserManager userManager, ApplicationRoleManager roleManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Index(string returnUrl)
		{

			// persistent admin account and role for development
			// because db will be dropped quite often
#if DEBUG
			string name = "admin@admin.lt";
			var admin = _userManager.Users.FirstOrDefault(x => x.UserName == name);

			if (admin == null)
			{
				var user = new ApplicationUser() { UserName = name, Email = name };
				var result = _userManager.Create(user, "qwerty");
				if (result.Succeeded)
				{
					_roleManager.Create(new IdentityRole("Admin"));
					_roleManager.Create(new IdentityRole("Manager"));
					_userManager.AddToRole(user.Id, "Admin");
				}
			}


#endif
			ViewBag.ReturnUrl = returnUrl;
			return View("Login");
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
				return View(model);

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		#region captcha etc
		////
		//// GET: /Account/VerifyCode
		//[AllowAnonymous]
		//public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		//{
		//	// Require that the user has already logged in via username/password or external login
		//	if (!await _signInManager.HasBeenVerifiedAsync())
		//		return View("Error");
		//	return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		//}

		////
		//// POST: /Account/VerifyCode
		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		//{
		//	if (!ModelState.IsValid)
		//		return View(model);

		//	// The following code protects for brute force attacks against the two factor codes. 
		//	// If a user enters incorrect codes for a specified amount of time then the user account 
		//	// will be locked out for a specified amount of time. 
		//	// You can configure the account lockout settings in IdentityConfig
		//	var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe,
		//		model.RememberBrowser);
		//	switch (result)
		//	{
		//		case SignInStatus.Success:
		//			return RedirectToLocal(model.ReturnUrl);
		//		case SignInStatus.LockedOut:
		//			return View("Lockout");
		//		case SignInStatus.Failure:
		//		default:
		//			ModelState.AddModelError("", "Invalid code.");
		//			return View(model);
		//	}
		//}

		////
		//// GET: /Account/ForgotPassword
		//[AllowAnonymous]
		//public ActionResult ForgotPassword()
		//{
		//	return View();
		//}

		////
		//// POST: /Account/ForgotPassword
		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		var user = await _userManager.FindByNameAsync(model.Email);
		//		if (user == null || !await _userManager.IsEmailConfirmedAsync(user.Id))
		//			return View("ForgotPasswordConfirmation");

		//		// For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
		//		// Send an email with this link
		//		// string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
		//		// var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
		//		// await _userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
		//		// return RedirectToAction("ForgotPasswordConfirmation", "Account");
		//	}

		//	// If we got this far, something failed, redisplay form
		//	return View(model);
		//}

		////
		//// GET: /Account/ForgotPasswordConfirmation
		//[AllowAnonymous]
		//public ActionResult ForgotPasswordConfirmation()
		//{
		//	return View();
		//}

		////
		//// GET: /Account/ResetPassword
		//[AllowAnonymous]
		//public ActionResult ResetPassword(string code)
		//{
		//	return code == null ? View("Error") : View();
		//}

		////
		//// POST: /Account/ResetPassword
		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		//{
		//	if (!ModelState.IsValid)
		//		return View(model);
		//	var user = await _userManager.FindByNameAsync(model.Email);
		//	if (user == null)
		//		return RedirectToAction("ResetPasswordConfirmation", "Login");
		//	var result = await _userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
		//	if (result.Succeeded)
		//		return RedirectToAction("ResetPasswordConfirmation", "Login");
		//	AddErrors(result);
		//	return View();
		//}

		////
		//// GET: /Account/ResetPasswordConfirmation
		//[AllowAnonymous]
		//public ActionResult ResetPasswordConfirmation()
		//{
		//	return View();
		//}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}
		#endregion
		#region Helpers

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError("", error);
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
					properties.Dictionary[XsrfKey] = UserId;
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		#endregion
	}
}