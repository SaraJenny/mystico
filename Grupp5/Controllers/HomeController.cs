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

		public List<Temp> Temp(string id)
		{
			// TODO hämta vänner som matchar string id

			var viewModel = new List<Temp>
			{
				new Temp { Id = 1, FirstName = "Kalle", LastName = "Kula", Email = "kalle@kula.se" },
				new Temp { Id = 2, FirstName = "Mimmi", LastName = "Mus", Email = "mimmi@mus.se" },
				new Temp { Id = 3, FirstName = "Musse", LastName = "Pigg", Email = "musse@pigg.se" },
			};

			return viewModel;
		}
	}
}
