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
			var model = new SplitEventVM();
			model.CurrencyItem = new SelectListItem[]
			{
				new SelectListItem { Text = "SEK", Value = "SEK"},
				new SelectListItem { Text = "NOK", Value = "NOK"},
				new SelectListItem { Text = "USD", Value = "USD"},
				new SelectListItem { Text = "EUR", Value = "EUR"}
			};

			return View(model);
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
            return View();
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
