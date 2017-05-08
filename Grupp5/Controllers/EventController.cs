using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Grupp5.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
			return Content("Du är inloggad");
        }
    }
}
