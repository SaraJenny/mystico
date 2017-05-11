using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Grupp5.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grupp5.Models.SplitModels;

namespace Grupp5.Controllers
{
    [Authorize]
    public class SplitController : Controller
    {
        MysticoContext mysticoContext;

        public SplitController(MysticoContext mysticoContext)
        {
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
        public IActionResult Index()
        {
            //Get all events by userId
            //omvandla events till split/indexVM
            //retunera view med array av splitindexVM

            mysticoContext.GetEventsByUserId(11);

            return View();
        }
        #endregion

        #region Overview

        public IActionResult Overview() // TODO lägg till int id som parameter
        {
			//Kika på ID och hämta event
			//Skicka Event till WhoOweWho()
			//Printa ut resultat på view..
			//var thisEvent = mysticoContext.GetEventById(id);

			//var message = "";
			//var listMessage = Library.WhoOweWho(thisEvent);


			//foreach (var item in listMessage)
			//{
			//    message += item;
			//}

			//return Content(message);
			return View();
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
