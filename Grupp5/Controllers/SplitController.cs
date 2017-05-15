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
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            if (mysticoContext.CheckIfUserIsParticipant(user.Id, id) == false)
                return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

            var expenses = mysticoContext.GetExpensesByEvent(id);
            var objections = mysticoContext.GetObjectionsInEvent(id);
            List<SplitDetailsVM> viewModel = Library.ConvertExpenseToSplitDetailsVM(expenses, objections);
            ViewBag.EventName = mysticoContext.GetEventById(id).EventName;

            return View(viewModel);
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

            mysticoContext.AddLoggedInUserToEvent(user.Id, newEvent.Id);
            mysticoContext.AddParticipantsToEvent(viewModel.FriendIds, newEvent.Id);

            return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = newEvent.Id });
        }
        #endregion

        #region EventHistory
        public async Task<IActionResult> EventHistory()
        {

            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);
            var myEvents = mysticoContext.GetInactiveEventsByUserId(user.Id);
            var myListofEvents = Library.ConvertToListOfEventsVMArray(myEvents);

            return View(myListofEvents);
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
            var myEvents = mysticoContext.GetActiveEventsByUserId(currentUser.Id);

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
            var myEvents = mysticoContext.GetActiveEventsByUserId(user.Id);
            var myListOfEvents = Library.ConvertToListOfEventsVMArray(myEvents);

            return View(myListOfEvents);
        }
        #endregion

        #region Overview

        [HttpGet]
        public async Task<IActionResult> Overview(int id) // TODO lägg till int id som parameter
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            if(mysticoContext.CheckIfUserIsParticipant(user.Id, id) == false)
                return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

            var thisEvent = mysticoContext.GetEventById(id);

            var transactions = Library.WhoOweWho(thisEvent);
            var transactionsVM = Library.ConvertWhoOwesWho(transactions);
            var myTransactions = new List<WhoOwesWhoVM>();
            var restTransactions = new List<WhoOwesWhoVM>();
            foreach (var item in transactionsVM)
            {
                if (item.DebitorId == user.Id || item.CreditorId == user.Id)
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
                Total = Library.GetTotalCostForEvent(thisEvent),
                EventIsActive = thisEvent.IsActive,
                EventId = id
                
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Overview(SplitOverviewVM viewModel, int id) // TODO lägg till int id som parameter
        {
            if (!ModelState.IsValid)
            {
                viewModel.Message = "Modelstate is not valid...";
                return View(viewModel);
            }

            mysticoContext.AddParticipantsToEvent(viewModel.FriendIds, id);
            viewModel.FriendIds = "";
            viewModel.Message = "Vänner tillagda! :)";
            //TODO viewModel.Message försvinner ju... kanske inte redirecta...
            return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = id });

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
