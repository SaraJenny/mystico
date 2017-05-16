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
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Grupp5.Controllers
{
    [Authorize]
    public class JsonController : Controller
    {

        #region General
        UserManager<IdentityUser> userManager;
        IdentityDbContext identityContext;
        MysticoContext mysticoContext;

        public JsonController(UserManager<IdentityUser> userManager, IdentityDbContext identityContext, MysticoContext mysticoContext)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.mysticoContext = mysticoContext;
        }
        #endregion

        #region GetAllUsers
        public List<UserVM> GetAllUsers()
        {
            //Add participants as userVM(as Json)
            var users = mysticoContext.GetAllUsers();
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return userVMs;
        }
        #endregion

        #region GetAllUsersExceptMe
        public async Task<List<UserVM>> GetAllUsersExceptMe()
        {
            //Send all users except current user
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            var users = mysticoContext.GetAllUsersExceptMe(user.Id);
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return userVMs;
        }
        #endregion

        #region GetUsersByEventId

        public List<UserVM> GetUsersByEventId(int id)
        {
            List<User> users = mysticoContext.GetUsersByEventId(id);
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return userVMs;
        }

        #endregion

        #region SearchUserExceptMe
        public async Task<List<UserVM>> SearchUserExceptMe(string search, string chosen)
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            var users = mysticoContext.SearchUserExceptMe(user.Id, search, chosen);
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return userVMs;
        }
        #endregion

        #region SearchAllUsersExceptAlreadyParticipantsAndChosen

        public List<UserVM> SearchAllUsersExceptAlreadyParticipantsAndChosen(string search, string chosen, string eventid)
        {

            var users = mysticoContext.SearchUserExceptParticipants(search, chosen, Convert.ToInt32(eventid));
            List<UserVM> userVMs = Library.ConvertUsersToUsersVM(users);

            return userVMs;
        }

        #endregion

        #region GetStandardCurrencyByEvent
        public int GetStandardCurrencyByEvent(int id)
        {
            var myEvent = mysticoContext.GetEventById(id);

            return myEvent.StandardCurrencyId;
        }
#endregion

    }
}
