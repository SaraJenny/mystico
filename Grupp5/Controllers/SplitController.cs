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
        #region General (context + constructor)
        UserManager<IdentityUser> userManager;
        IdentityDbContext identityContext;
        MysticoContext mysticoContext;

        public SplitController(UserManager<IdentityUser> userManager, IdentityDbContext identityContext, MysticoContext mysticoContext)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.mysticoContext = mysticoContext;
        }
#endregion

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
            viewModel.CurrencyItem = Library.ConvertCurrencyToSelectListItem(allCurrencies);

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

            return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = newEvent.Id });
        }
        #endregion

        #region EventHistory
        public IActionResult EventHistory()
        {
            return View();
        }
        #endregion

        #region Expense
        [HttpGet]
        public async Task<IActionResult> Expense()
        {
            //Hämta currentUser
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User currentUser = mysticoContext.GetUserByAspUserId(myUser.Id);

            //Hämta event från databasen som currentUser är med i...
            var myEvents = mysticoContext.GetEventsByUserId(currentUser.Id);

            //Hämta valutor från databasen
            List<Currency> allCurrencies = mysticoContext.GetAllCurrencies();

            var viewModel = new SplitExpenseVM();
            viewModel.CurrencyItem = Library.ConvertCurrencyToSelectListItem(allCurrencies);
            viewModel.EventItem = Library.ConvertEventToSelectListItem(myEvents);
            viewModel.Date = DateTime.Today.ToString().Replace(" 00:00:00", "");

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Expense(SplitExpenseVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            var expenseId = mysticoContext.CreateExpense(viewModel, user.Id);

            mysticoContext.CreatePayerForExpense(viewModel.FriendIds, expenseId);

            return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = viewModel.SelectedEvent });
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

            if(mysticoContext.CheckIfUserIsParticipant(user.Id, id) == false)
                return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

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
