using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Grupp5.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grupp5.Controllers
{
    [Authorize]
    public class SplitController : Controller
    {
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
            return View();
        }
        #endregion

        #region Overview

        public IActionResult Overview()
        {
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
