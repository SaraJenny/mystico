using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Grupp5.Models.Entities;
using Grupp5.Models.SplitModels;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Grupp5.Controllers
{
    public class JsonController : Controller
    {

        UserManager<IdentityUser> userManager;
        IdentityDbContext identityContext;
        MysticoContext mysticoContext;

        public JsonController (UserManager<IdentityUser> userManager, IdentityDbContext identityContext, MysticoContext mysticoContext)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.mysticoContext = mysticoContext;
        }

        public List<UserVM> GetAllUsers()
        {
            //Add participants as userVM(as Json)
            var users = mysticoContext.GetAllUsers();
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return userVMs;
        }


        public async Task<IActionResult> GetAllUsersExceptMe()
        {
            //Send all users except current user
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            var users = mysticoContext.GetAllUsersExceptMe(user.Id);
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return Json(JsonConvert.SerializeObject(userVMs));
        }
    }
}
