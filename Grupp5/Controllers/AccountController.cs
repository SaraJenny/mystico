using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Grupp5.Models.Entities;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> Login()
        {
            //identityContext.Database.EnsureCreated();
            //TODO check if user is logged in already

            var myUser = await userManager.GetUserAsync(HttpContext.User);
            if (myUser != null)
                return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

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
                ModelState.AddModelError(nameof(AccountLoginVM.Password), "Ogiltig inloggning");
                return View(viewModel);
            }

            return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

        }
        #endregion

        #region Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            if (myUser != null)
                return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

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

            try
            {
                mysticoContext.UpdateUserProfile(viewModel, user);

                var resultUserName = await userManager.SetUserNameAsync(myUser, viewModel.Email);

                if (viewModel.Password != null)
                {
                    var resultPassWord = await userManager.ChangePasswordAsync(myUser, viewModel.CurrentPassword, viewModel.Password);
                }

                viewModel.Message = "Du har uppdaterat din profil";
            }
            catch
            {
                viewModel.Message = "Knas!";
            }
            finally
            {
                viewModel.Password = "";
                viewModel.CurrentPassword = "";
                viewModel.PasswordCheck = "";
            }
            return View(viewModel);
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

        #region Google
        
        //TODO fixa snyggare redirect etc..
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }


        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return RedirectToAction(nameof(Login));
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal("/split");
            }
            if (result.IsLockedOut)
            {
                return Content("Lockout");
            }
            else
            {
                //Registrerar användaren automatiskt om det är första gången man loggar in
                var email = info.Principal.Claims.SingleOrDefault(x => x.Type.Contains("emailaddress"))?.Value;
                var firstName = info.Principal.Claims.SingleOrDefault(x => x.Type.Contains("givenname"))?.Value;
                var lastName = info.Principal.Claims.SingleOrDefault(x => x.Type.Contains("surname"))?.Value;

                var user = new IdentityUser { UserName = email, Email = email };

                await userManager.CreateAsync(user);

                await userManager.AddLoginAsync(user, info);

                await signInManager.SignInAsync(user, false);

                mysticoContext.AddUser(user.Id, firstName, lastName, email);

                return RedirectToLocal("/split");
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }
}
