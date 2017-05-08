using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Grupp5.Models.Entities;

namespace Grupp5.Controllers
{
	public class AccountController : Controller
	{
		UserManager<IdentityUser> userManager;
		SignInManager<IdentityUser> signInManager;
		IdentityDbContext identityContext;

		public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IdentityDbContext identityContext)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.identityContext = identityContext;
		}

		[HttpGet]
		public IActionResult Login()
		{
			//identityContext.Database.EnsureCreated();
			//TODO check if user is logged in already

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(AccountLoginVM viewModel)
		{
			if (!ModelState.IsValid)
				return View(viewModel);
			

			var result = await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, false);

			if (!result.Succeeded)
			{
				ModelState.AddModelError(nameof(AccountLoginVM.Email), "Ogiltig inloggning");
				return View(viewModel);
			}

			return RedirectToAction(nameof(EventController.Index), nameof(EventController).Replace("Controller", ""));

		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(AccountRegisterVM viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}

			var user = new IdentityUser();
			user.Email = viewModel.Email;
			user.UserName = viewModel.Email;
			var result = await userManager.CreateAsync(user, viewModel.Password);

			if (!result.Succeeded)
			{
				if (result.ToString().Contains("DuplicateEmail"))
				{
					ModelState.AddModelError(nameof(AccountRegisterVM.Email), "E-posten är redan registrerad");
				}

				if (result.ToString().Contains("DuplicateUserName"))
				{
					ModelState.AddModelError(nameof(AccountRegisterVM.Email), "E-posten är redan registrerad");
				}
				return View(viewModel);
			}

			await signInManager.PasswordSignInAsync(user, viewModel.Password, false, false);

			return RedirectToAction(nameof(EventController.Index), nameof(EventController).Replace("Controller", ""));
		}

	}
}
