using Grupp5.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public partial class MysticoContext : DbContext
    {
        public MysticoContext(DbContextOptions<MysticoContext> options) : base(options)
        {

        }

        public void AddUser(string id, string firstName, string lastName, string email)
        {
            try
            {
                User.Add(new User
                {
                    AspId = id,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                });

                SaveChanges();
            }
            catch (Exception x)
            {
                Debug.Write(x.Message);
            }
        }

        public Event GetEventById(int id)
        {
            var myEvent = Event.Where(e => e.Id == id).First();

            myEvent.ParticipantsInEvent = ParticipantsInEvent.Where(p => p.EventId == myEvent.Id).ToList();

            myEvent.Expense = Expense.Where(e => e.EventId == myEvent.Id).ToList();

            foreach (var expense in myEvent.Expense)
            {
                expense.PayersForExpense = PayersForExpense.Where(p => p.ExpenseId == expense.Id).ToList();
            }

            foreach (var person in myEvent.ParticipantsInEvent)
            {
                person.User = User.Where(u => u.Id == person.UserId).First();
            }

            return myEvent;
        }

        internal List<PayersForExpense> GetObjectionsInEvent(int id)
        {
            return PayersForExpense.Where(p => p.Expense.EventId == id && p.Objection == true).ToList();
        }

        internal List<Expense> GetExpensesByEvent(int eventId)
        {
            var expenses = Expense.Where(e => e.EventId == eventId).ToList();
            foreach (var item in expenses)
            {
                item.PayersForExpense = PayersForExpense.Where(p => p.ExpenseId == item.Id).ToList();
                foreach (var payer in item.PayersForExpense)
                {
                    payer.User = User.Where(u => u.Id == payer.UserId).First();
                }
                item.Currency = Currency.Where(c => c.Id == item.CurrencyId).First();
            }
           
            return expenses;
        }

        internal List<User> GetAllUsers()
        {
            return User.ToList();
        }

        internal List<User> GetAllUsersExceptMe(int id)
        {
            return User.Where(u => u.Id != id).ToList();
        }

        public List<Currency> GetAllCurrencies()
        {
            return Currency.ToList();
        }

        public Event CreateEvent(SplitEventVM viewModel)
        {

            var newEvent = new Event();
            newEvent.EventName = viewModel.Name;
            newEvent.Description = viewModel.Description;
            newEvent.IsActive = true;
            newEvent.StandardCurrencyId = Convert.ToInt32(viewModel.SelectedCurrency);

            Event.Add(newEvent);
            SaveChanges();

            return newEvent;
        }

        internal List<User> GetUsersByEventId(int id)
        {
            var participants = ParticipantsInEvent.Where(e => e.EventId == id).ToList();
            var users = new List<User>();

            foreach (var participant in participants)
            {
                users.Add(User.Where(u => u.Id == participant.UserId).First());
            }
            return users;
        }

        internal List<User> SearchUserExceptMe(int id, string search, string chosen)
        {
            List<string> chosenIds = new List<string>();

            try
            {
                chosenIds = chosen.Split(',').ToList();
            }
            catch(Exception ex)
            {
				Console.WriteLine(ex);
            }

            var users = new List<User>();

            //OM search matchar med User.Name osv... (firstname, lastname, email)
            var userMatching = User.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search) || u.Email.Contains(search)).ToList();

            foreach (var user in userMatching)
            {
                //KOlla så att id inte matcher med mitt id & chosenId..
                //OM BÅDA stämmer (check på första, ej på andra) => Lägg till i users
                if (user.Id != id &&
                    chosenIds.Where(u => Convert.ToInt32(u) == user.Id).Count() == 0)
                    users.Add(user);
            }

            return users;
        }

        public void AddParticipantsToEvent(string friends, int eventId, int currentUserId)
        {
           var FriendIds = friends.Split(',');

            ParticipantsInEvent.Add(new ParticipantsInEvent
            {
                EventId = eventId,
                UserId = currentUserId,
            });

            foreach (var userId in FriendIds)
            {
                if (Convert.ToInt32(userId) != currentUserId)
                {
                    ParticipantsInEvent.Add(new ParticipantsInEvent
                    {
                        EventId = eventId,
                        UserId = Convert.ToInt32(userId)
                    });
                }
            };

            SaveChanges();
        }

        public void UpdateUserProfile(AccountProfileVM viewModel, User user)
        {
            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.Email = viewModel.Email;

            SaveChanges();
        }

        public List<Event> GetEventsByUserId(int id)
        {
            var myEvents = ParticipantsInEvent.Where(p => p.UserId == id).Select(p => p.Event).ToList();
            return myEvents;
        }

        public User GetUserByAspUserId(string id)
        {
            return User.Where(u => u.AspId == id).First();
        }

        internal int CreateExpense(SplitExpenseVM viewModel, int currentUserId)
        {
            var newExpense = new Expense()
            {

                Amount = Convert.ToDecimal(viewModel.Amount),
                Description = viewModel.Description,
                CurrencyId = Convert.ToInt32(viewModel.SelectedCurrency),
                Date = Convert.ToDateTime(viewModel.Date),
                PurchaserId = currentUserId,
                EventId = Convert.ToInt32(viewModel.SelectedEvent),
                AmountInStandardCurrency = Convert.ToDecimal(viewModel.Amount) //TODO valutaomvandling

            };

            Expense.Add(newExpense);
            SaveChanges();
            return newExpense.Id;

        }

        internal void CreatePayerForExpense(string payerIds, int expenseId)
        {
            var splitpayers = payerIds.Split(',');

            foreach (var payerId in splitpayers)
            {
                PayersForExpense.Add(new PayersForExpense
                {
                    ExpenseId = expenseId,
                    UserId = Convert.ToInt32(payerId),
                    Objection = false
                });
            }

            SaveChanges();
        }

        internal bool CheckIfUserIsParticipant(int userId, int eventId)
        {
            if (ParticipantsInEvent.Where(p => p.UserId == userId && p.EventId == eventId).Count() > 0)
                return true;
            else
                return false;
        }
    }
}
