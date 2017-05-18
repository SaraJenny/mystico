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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;

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
        IConfiguration iConfiguration;

        public JsonController(UserManager<IdentityUser> userManager, IdentityDbContext identityContext, MysticoContext mysticoContext, IConfiguration iConfiguration)
        {
            this.userManager = userManager;
            this.identityContext = identityContext;
            this.mysticoContext = mysticoContext;
            this.iConfiguration = iConfiguration;
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

        #region GetExpenseCurrencyByEvent
        public int GetExpenseCurrencyByEvent(int id)
        {
            var myEvent = mysticoContext.GetEventById(id);

            return myEvent.ExpenseCurrencyId;
        }
        #endregion

        #region GetUsersByExpense
        public List<PayerVM> GetUsersByExpense(int id)
        {
            var myExpense = mysticoContext.GetExpenseById(id);
            var myEvent = mysticoContext.GetEventByExpense(myExpense);

            var myParticipants = mysticoContext.GetUsersByEventId(myEvent.Id);
            var myPayers = mysticoContext.GetPayersByExpense(myExpense);

            var participantsWithMarkedPayers = Library.ConvertToListOfPayersVM(myParticipants, myPayers);

            return participantsWithMarkedPayers;
        }
        #endregion

        #region GetPossiblePurchaserById

        public async Task<SelectListItem[]> GetPossiblePurchaserById(int id)
        {
            var myUser = await userManager.GetUserAsync(HttpContext.User);
            User user = mysticoContext.GetUserByAspUserId(myUser.Id);

            var myEvent = mysticoContext.GetEventById(id);
            var possiblePurchasers = Library.ConvertParticipantsToSelectListItem(myEvent, user.Id);

            return possiblePurchasers;
        }

        #endregion

        #region AddUsersToEvent
        public bool AddUsersToEvent(string userIds, int eventId)
        {
            try
            {
                mysticoContext.AddParticipantsToEvent(userIds, eventId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region ForgotPassword

        [AllowAnonymous]
        public async Task<bool> ForgotPassword(string email = null)
        {
            if (email != null)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    Random random = new Random();
                    Thread.Sleep(random.Next(500, 1000));
                    // Don't reveal that the user does not exist or is not confirmed
                    return true;
                }

                SendEmailAsync(email, user);

                return true;
            }

            // If we got this far, something failed, redisplay form
            return false;
        }

        #endregion

        #region SendEmail
        private async void SendEmailAsync(string email, IdentityUser user)
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(AccountController.ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

            var messages = new MimeMessage();
            messages.From.Add(new MailboxAddress("Payme", "Payme_Academy@outlook.com"));
            messages.To.Add(new MailboxAddress("", email));
            messages.Subject = "Återställ lösenord";
            messages.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<h2>Hej din guldfisk!</h2>" +
                $"<p>Någon har anmält att du har glömt bort ditt lösenord hos oss på PayMe. Det kan väl inte stämma?</p>" +
                $"<a href='{callbackUrl}'>Klicka här om du vill återställa ditt lösenord.</a>" +
                $"<p>Vi saknar dig!</p>" +
                $"<p>/PayMe-teamet</p>"

            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp-mail.outlook.com", 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate("Payme_Academy@outlook.com", iConfiguration["EmailPassWord"]);

                // Note: since we don't have an OAuth2 token, disable     // the XOAUTH2 authentication mechanism.    
                await client.SendAsync(messages);
                client.Disconnect(true);
            }

        }

        #endregion

        #region GetPossiblePurchaserByIdEditVersion

        public SelectListItem[] GetPossiblePurchaserByIdEditVersion(int eventId, int expenseId)
        {
            var myEvent = mysticoContext.GetEventById(eventId);
            var myExpense = mysticoContext.GetExpenseById(expenseId);

            var possiblePurchasers = Library.ConvertParticipantsToSelectListItemEditVersion(myEvent, myExpense);

            return possiblePurchasers;
        }

        #endregion

    }
}
