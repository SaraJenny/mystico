using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grupp5.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Grupp5.Controllers
{
    public class HomeController : Controller
    {
        #region General

        UserManager<IdentityUser> userManager;

        public HomeController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        #endregion

        #region HttpError

        public IActionResult HttpError(int id)
        {
            // Visa upp vyn "HttpError" i Views/Shared return View(id);
            return View(id);

        }

        #endregion

        #region ServerError
        public IActionResult ServerError()
        {
            // Visa upp vyn "ServerError" i Views/Shared return View();
            return View();
        }

        #endregion

        #region About
        public async Task<IActionResult> About()
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);

            if (myUser != null)
                ViewBag.LoggedIn = true;
            else
                ViewBag.LoggedIn = false;
            return View();
        }
        #endregion

        #region Index
        public IActionResult Index()
        {


            return View();
        }
        #endregion

        #region Logo
        public IActionResult Logo()
        {
            return View();
        }
        #endregion

    }
}
