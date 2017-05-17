using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grupp5.Models.Entities;

namespace Grupp5.Controllers
{
    public class HomeController : Controller
    {

        #region About
        public IActionResult About()
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

		#region Logo
		public IActionResult Logo()
		{
			return View();
		}
		#endregion
	}
}
