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
        MysticoContext mysticoContext;

        public HomeController(MysticoContext mysticoContext)
        {
            this.mysticoContext = mysticoContext;
        }
        #region About
        public IActionResult About()
        {
            return View();
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            await mysticoContext.CalculateStandardCurrencyAmount(500,1,1,DateTime.Now);

            return View();
        }
		#endregion

	}
}
