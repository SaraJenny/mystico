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
			expenses = expenses.OrderByDescending(e => e.Date).ToList();

			var myEvent = mysticoContext.GetEventById(id);

			var objections = mysticoContext.GetObjectionsInEvent(id);
			List<SplitDetailsVM> viewModel = Library.ConvertExpenseToSplitDetailsVM(expenses, objections);
			ViewBag.CurrentUserId = user.Id;
			ViewBag.EventName = myEvent.EventName;
			ViewBag.IsActive = myEvent.IsActive;

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
		public async Task<IActionResult> Expense(int id)
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
            viewModel.Date = DateTime.Today.ToString("u").Replace(" 00:00:00Z", "");

            if (id != 0)
                viewModel.SelectedEvent = id;

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Expense(SplitExpenseVM viewModel)
		{
			if (!ModelState.IsValid)
				return View(viewModel);

			var expenseId = await mysticoContext.CreateExpense(viewModel);

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
		public async Task<IActionResult> Overview(int id)
		{
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User user = mysticoContext.GetUserByAspUserId(myUser.Id);

			if (mysticoContext.CheckIfUserIsParticipant(user.Id, id) == false)
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

			List<UserVM> participants = Library.ConvertUsersToUsersVM(mysticoContext.GetUsersByEventId(thisEvent.Id));

			var viewModel = new SplitOverviewVM()
			{
				TransactionsCommittedToMe = myTransactions,
				TransactionsWithoutMe = restTransactions,
				EventName = thisEvent.EventName,
				MyStatus = Library.GetUserStatusById(thisEvent, user.Id),
				MyTotal = Library.GetUserTotalById(thisEvent, user.Id),
				Total = Library.GetTotalCostForEvent(thisEvent),
				EventIsActive = thisEvent.IsActive,
				EventId = id,
				StandardCurrency = thisEvent.StandardCurrency.CurrencyCode,
				AlreadyParticipants = participants
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

		#region UpdateEvent

		[HttpGet]
		public async Task<IActionResult> UpdateEvent(int id)
		{
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User user = mysticoContext.GetUserByAspUserId(myUser.Id);

			var myEvent = mysticoContext.GetEventById(id);
			if (myEvent.ParticipantsInEvent.Where(p => p.UserId == user.Id).Count() == 0)
				return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

			var viewModel = Library.ConvertEventToSplitEventVM(myEvent);
			List<Currency> allCurrencies = mysticoContext.GetAllCurrencies();
			viewModel.CurrencyItem = Library.ConvertCurrencyToSelectListItem(allCurrencies);


			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateEvent(int id, SplitEventVM viewModel)
		{
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User user = mysticoContext.GetUserByAspUserId(myUser.Id);

			var myEvent = mysticoContext.GetEventById(id);
			if (myEvent.ParticipantsInEvent.Where(p => p.UserId == user.Id).Count() == 0)
				return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = myEvent.Id });

			await mysticoContext.UpdateEvent(myEvent, viewModel);
			mysticoContext.AddParticipantsToEvent(viewModel.FriendIds, id);

			return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = myEvent.Id });
		}

		#endregion

		#region DeleteEvent

		public async Task<IActionResult> DeleteEvent(int id)
		{
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User user = mysticoContext.GetUserByAspUserId(myUser.Id);

			var myEvent = mysticoContext.GetEventById(id);
			if (myEvent.ParticipantsInEvent.Where(p => p.UserId == user.Id).Count() == 0)
				return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));

			mysticoContext.InActivateEvent(myEvent);

			return RedirectToAction(nameof(SplitController.Index), nameof(SplitController).Replace("Controller", ""));
		}

		#endregion

		#region UpdateExpense
		[HttpGet]
		public async Task<IActionResult> UpdateExpense(int id)
		{
			//Hämta currentUser
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User currentUser = mysticoContext.GetUserByAspUserId(myUser.Id);

			//Hämta specifikt event som ska ändras
			var myExpense = mysticoContext.GetExpenseById(id);

			//OM expense inte finns eller (jag inte är inköpare eller det är en fakeperson)  ==> skickas till overview
			if (myExpense == null || (myExpense.PurchaserId != currentUser.Id && (myExpense.PurchaserId > 47 && myExpense.PurchaserId < 32)))
				return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""));

			//Hämta event från databasen som currentUser är med i...
			var myEvents = mysticoContext.GetActiveEventsByUserId(currentUser.Id);

			//Hämta valutor från databasen
			List<Currency> allCurrencies = mysticoContext.GetAllCurrencies();
			List<User> payers = mysticoContext.GetPayersByExpense(myExpense);

			var viewModel = Library.ConvertToSplitExpenseVM(myExpense);
			viewModel.CurrencyItem = Library.ConvertCurrencyToSelectListItem(allCurrencies);
			viewModel.EventItem = Library.ConvertEventToSelectListItem(myEvents);
			viewModel.Payers = Library.ConvertUsersToUsersVM(payers);
			viewModel.ExpenseId = myExpense.Id;


			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateExpense(SplitExpenseVM viewModel, int id)
		{

			if (!ModelState.IsValid)
				return View(viewModel);

			//Hämta currentUser
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User currentUser = mysticoContext.GetUserByAspUserId(myUser.Id);

			//Hämta specifikt expense som ska ändras
			var myExpense = mysticoContext.GetExpenseById(id);

			//OM jag inte är inköpare ==> skickas till overview
			if (myExpense.PurchaserId != currentUser.Id && (myExpense.PurchaserId < 32 || myExpense.PurchaserId > 47))
				return RedirectToAction(nameof(SplitController.Details), nameof(SplitController).Replace("Controller", ""), new { id = myExpense.EventId });

			await mysticoContext.UpdateExpense(myExpense, viewModel);

			return RedirectToAction(nameof(SplitController.Details), nameof(SplitController).Replace("Controller", ""), new { id = myExpense.EventId });

		}

		#endregion

		#region DeleteExpense
		[HttpGet]
		public async Task<IActionResult> DeleteExpense(int id)
		{
			var myUser = await userManager.GetUserAsync(HttpContext.User);
			User user = mysticoContext.GetUserByAspUserId(myUser.Id);

			Expense myExpense = mysticoContext.GetExpenseById(id);
			if (myExpense == null || myExpense.PurchaserId != user.Id)
				return RedirectToAction(nameof(SplitController.Details), nameof(SplitController).Replace("Controller", ""), new { id = myExpense.EventId });

			mysticoContext.DeleteExpense(myExpense);

			return RedirectToAction(nameof(SplitController.Details), nameof(SplitController).Replace("Controller", ""), new { id = myExpense.EventId });
		}
		#endregion

		#region ReActivateEvent
		public IActionResult ReActivateEvent(int id)
		{
			mysticoContext.ReActivateEvent(id);

			return RedirectToAction(nameof(SplitController.Overview), nameof(SplitController).Replace("Controller", ""), new { id = id });
		}
		#endregion

	}
}
