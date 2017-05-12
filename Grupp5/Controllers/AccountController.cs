using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Grupp5.Models.Entities;
using Grupp5.Models.ViewModels;

namespace Grupp5.Controllers
{
    public class AccountController : Controller
    {
        #region General
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identityContext;
        MysticoContext mysticoContext;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IdentityDbContext identityContext, MysticoContext mysticoContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identityContext = identityContext;
            this.mysticoContext = mysticoContext;
        }
        #endregion

        #region Login
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

            return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

        }
        #endregion

        #region Register
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

            mysticoContext.AddUser(user.Id, viewModel.FirstName, viewModel.LastName, viewModel.Email);

            await signInManager.PasswordSignInAsync(user, viewModel.Password, false, false);

            return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));
        }
        #endregion

        #region Profile
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            AccountProfileVM viewModel = new AccountProfileVM()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Profile(AccountProfileVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            //Updater vår userProfile
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            mysticoContext.UpdateUserProfile(viewModel, user);

            var resultUserName = await userManager.SetUserNameAsync(myUser, viewModel.Email);
            
            if (viewModel.Password != null)
            {
                var resultPassWord = await userManager.ChangePasswordAsync(myUser, viewModel.CurrentPassword, viewModel.Password);
            }


            return View();
        }
        #endregion

        #region SignOut

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
        }

        #endregion
    }
}
