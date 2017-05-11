using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Grupp5.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grupp5.Models.SplitModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace Grupp5.Controllers
{
    [Authorize]
    public class SplitController : Controller
    {
        UserManager<IdentityUser> userManager;
        IdentityDbContext identityContext;
        MysticoContext mysticoContext;

        public SplitController(UserManager<IdentityUser> userManager, IdentityDbContext identityContext, MysticoContext mysticoContext)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.mysticoContext = mysticoContext;
        }

        #region Details
        public IActionResult Details()
        {
            return View();
        }
        #endregion

        #region Event
        public IActionResult Event()
        {
			// TODO Hämta valutor från databasen
			var viewModel = new SplitEventVM();
			viewModel.CurrencyItem = new SelectListItem[]
			{
				new SelectListItem { Text = "SEK", Value = "SEK"},
				new SelectListItem { Text = "NOK", Value = "NOK"},
				new SelectListItem { Text = "USD", Value = "USD"},
				new SelectListItem { Text = "EUR", Value = "EUR"}
			};

			return View(viewModel);
        }
        #endregion

        #region EventHistory
        public IActionResult EventHistory()
        {
            return View();
        }
        #endregion

        #region Expense
        public IActionResult Expense()
        {
			// TODO Hämta event från databasen
			var viewModel = new SplitExpenseVM();
			viewModel.EventItem = new SelectListItem[]
			{
				new SelectListItem { Text = "Londonresa", Value = "1"},
				new SelectListItem { Text = "Maj 2017", Value = "2"}
			};

			// TODO Hämta valutor från databasen
			viewModel.CurrencyItem = new SelectListItem[]
			{
				new SelectListItem { Text = "SEK", Value = "SEK"},
				new SelectListItem { Text = "NOK", Value = "NOK"},
				new SelectListItem { Text = "USD", Value = "USD"},
				new SelectListItem { Text = "EUR", Value = "EUR"}
			};

			return View(viewModel);
		}
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);
            var myEvents = mysticoContext.GetEventsByUserId(user.Id);
            var mySplitIndexVm = Library.ConvertToSplitIndexVMArray(myEvents);
            
            return View(mySplitIndexVm);
        }
        #endregion

        #region Overview

        public IActionResult Overview(int id)
        {
            //Kika på ID och hämta event
            //Skicka Event till WhoOweWho()
            //Printa ut resultat på view..
            var thisEvent = mysticoContext.GetEventById(id);

            var message = "";
            var listMessage = Library.WhoOweWho(thisEvent);


            foreach (var item in listMessage)
            {
                message += item;
            }

            return Content(message);
        }
        #endregion

        #region Search
        public IActionResult Search()
        {
            return View();
        }
        #endregion
    }
}
