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
        [HttpGet]
        public IActionResult Event()
        {
            var viewModel = new SplitEventVM();
            List<Currency> allCurrencies = mysticoContext.GetAllCurrencies();
            viewModel.CurrencyItem = Library.ConvertCurrencyToSplitEventVMCurrencyItem(allCurrencies);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Event(SplitEventVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            Event newEvent = mysticoContext.CreateEvent(viewModel);
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

			var FriendIds = new List<int> { 12, 11, 13 };
			//var FriendIds = viewModel.FriendIds.Split(',');

			mysticoContext.AddParticipantsToEvent(FriendIds, newEvent.Id, user.Id);

            return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""),new {id = newEvent.Id });
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

        public async Task<IActionResult> Overview(int id) // TODO lägg till int id som parameter
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            var thisEvent = mysticoContext.GetEventById(id);

            var listMessage = Library.WhoOweWho(thisEvent);
            var myTransactions = new List<string>();
            var restTransactions = new List<string>();
            foreach (var item in listMessage)
            {
                if (item.Contains(user.FirstName))
                    myTransactions.Add(item);
                else
                    restTransactions.Add(item);
            }

            var viewModel = new SplitOverviewVM()
            {
                TransactionsCommittedToMe = myTransactions,
                TransactionsWithoutMe = restTransactions,
                EventName = thisEvent.EventName,
                MyStatus = Library.GetUserStatusById(thisEvent, user.Id),
                MyTotal = Library.GetUserTotalById(thisEvent, user.Id),
                Total = Library.GetTotalCostForEvent(thisEvent)
            };
            
 
            return View(viewModel);
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
