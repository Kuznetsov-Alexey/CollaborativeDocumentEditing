using DocumentEditing.Domain.Contracts;
using DocumentEditing.Domain.Contracts.Models;
using DocumentEditing.Domain.Contracts.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentEditing.Web.Controllers
{
	public class AccountController : Controller
	{
		private IMyUserManager _userManager;

		public AccountController(IMyUserManager userManager)
		{
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if(ModelState.IsValid)
			{
				var user = new ApplicationUserModel { Email = model.Email, Password = model.Password };

				var result = await _userManager.PasswordSignIn(user, model.RememberMe);
				if(result.Succeeded)
				{
					if(!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
					{
						return LocalRedirect(model.ReturnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Project");
					}
				}
				else
				{
					ModelState.AddModelError("", "Wrong Email and/or password");
				}
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _userManager.SignOut();

			return RedirectToAction("Login", "Account");
		}

		[HttpGet]
		public IActionResult Register()
		{
			ViewData["Title"] = "Register";
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if(ModelState.IsValid)
			{

				var user = new ApplicationUserModel {Email = model.Email, Password = model.Password, UserName = model.Email };

				var result = await _userManager.CreateUser(user);

				if(result.Succeeded)
				{
					await _userManager.SignIn(user);
					return RedirectToAction("Index", "Project");
				}
				else
				{
					foreach(var error in result.Errors)
					{
						ModelState.AddModelError(string.Empty, error.Description);
					}
				}
			}
			return View();
		}
	}
}
