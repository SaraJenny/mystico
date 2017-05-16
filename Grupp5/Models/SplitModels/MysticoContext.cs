using Grupp5.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{

    public partial class MysticoContext : DbContext
    {
        public async Task<decimal> CalculateStandardCurrencyAmount(decimal amount, int currencyId, int standardCurrencyId, DateTime date)
        {
            HttpClient httpClient = new HttpClient();

            var json = await httpClient.GetStringAsync("http://api.fixer.io/2015-05-15?symbols=USD&base=SEK");
            dynamic response = JsonConvert.DeserializeObject(json);
            //dynamic rates = response.rates;
            var currency = "USD";
            decimal value = response.rates[currency];
            //var ret = decimal.Parse(value,new CultureInfo ("en-US"));

            return value;
        }

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

            myEvent.StandardCurrency = Currency.Where(c => c.Id == myEvent.StandardCurrencyId).FirstOrDefault();

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

        internal List<User> SearchUserExceptParticipants(string search, string chosen, int eventId)
        {
            List<string> chosenIds = new List<string>();

            try
            {
                chosenIds = chosen.Split(',').ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var users = new List<User>();

            var participants = GetUsersByEventId(eventId);

            //OM search matchar med User.Name osv... (firstname, lastname, email)
            var userMatching = User.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search) || u.Email.Contains(search)).ToList();

            foreach (var user in userMatching)
            {
                //KOlla så att id inte matcher med mitt id & chosenId..
                //OM BÅDA stämmer (check på första, ej på andra) => Lägg till i users
                if (participants.Where(p => p.Id == user.Id).Count() == 0 &&
                    chosenIds.Where(u => Convert.ToInt32(u) == user.Id).Count() == 0)
                    users.Add(user);
            }

            return users;
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
            catch (Exception ex)
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

        public void AddParticipantsToEvent(string friends, int eventId)
        {

            try
            {
                var FriendIds = friends.Split(',');
                foreach (var userId in FriendIds)
                {
                    ParticipantsInEvent.Add(new ParticipantsInEvent
                    {
                        EventId = eventId,
                        UserId = Convert.ToInt32(userId)
                    });

                };

                SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public void AddLoggedInUserToEvent(int currentUserId, int eventId)
        {

            ParticipantsInEvent.Add(new ParticipantsInEvent
            {
                EventId = eventId,
                UserId = currentUserId,
            });
            SaveChanges();
        }

        public void UpdateUserProfile(AccountProfileVM viewModel, User user)
        {
            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.Email = viewModel.Email;

            SaveChanges();
        }

        public List<Event> GetActiveEventsByUserId(int id)
        {
            var myEvents = ParticipantsInEvent.Where(p => p.UserId == id && p.Event.IsActive == true).Select(p => p.Event).ToList();
            return myEvents;
        }

        public List<Event> GetInactiveEventsByUserId(int id)
        {
            var myEvents = ParticipantsInEvent.Where(p => p.UserId == id && p.Event.IsActive == false).Select(p => p.Event).ToList();
            return myEvents;
        }

        public User GetUserByAspUserId(string id)
        {
            return User.Where(u => u.AspId == id).First();
        }

        internal async Task<int> CreateExpense(SplitExpenseVM viewModel, int currentUserId)
        {
            var myEvent = GetEventById(Convert.ToInt32(viewModel.SelectedEvent));

            var newExpense = new Expense()
            {

                Amount = Convert.ToDecimal(viewModel.Amount),
                Description = viewModel.Description,
                CurrencyId = Convert.ToInt32(viewModel.SelectedCurrency),
                Date = Convert.ToDateTime(viewModel.Date),
                PurchaserId = currentUserId,
                EventId = myEvent.Id,
                AmountInStandardCurrency = await CalculateStandardCurrencyAmount(Convert.ToDecimal(viewModel.Amount), viewModel.SelectedCurrency, myEvent.StandardCurrencyId, Convert.ToDateTime(viewModel.Date)) //TODO valutaomvandling

            };

            Expense.Add(newExpense);
            SaveChanges();
            return newExpense.Id;

        }

        internal void InActivateEvent(Event myEvent)
        {
            myEvent.IsActive = false;
            SaveChanges();
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

        internal async Task UpdateEvent(Event myEvent, SplitEventVM viewModel)
        {
            myEvent.EventName = viewModel.Name;
            myEvent.Description = viewModel.Description;
            if (myEvent.StandardCurrencyId != Convert.ToInt32(viewModel.SelectedCurrency))
            {
                myEvent.StandardCurrencyId = Convert.ToInt32(viewModel.SelectedCurrency);

                SaveChanges();

                foreach (var expense in myEvent.Expense)
                {
                    await UpdateAmountInExpense(expense);
                }
            }


            SaveChanges();
        }

        private async Task UpdateAmountInExpense(Expense expense)
        {
            expense.AmountInStandardCurrency = await CalculateStandardCurrencyAmount(expense.Amount, expense.CurrencyId, expense.Event.StandardCurrencyId, expense.Date);
        }

        internal Expense GetExpenseById(int id)
        {
            var myExpense = Expense.Where(e => e.Id == id).FirstOrDefault();
            var listOfPayers = PayersForExpense.Where(p => p.ExpenseId == myExpense.Id).ToList();
            myExpense.PayersForExpense = listOfPayers;

            foreach (var payer in myExpense.PayersForExpense)
            {
                payer.User = User.Where(u => u.Id == payer.UserId).FirstOrDefault();
            }

            return myExpense;
        }

        internal void DeleteExpense(Expense myExpense)
        {
            foreach (var payer in myExpense.PayersForExpense)
            {
                PayersForExpense.Remove(payer);

            }
            SaveChanges();

            Expense.Remove(myExpense);

            SaveChanges();
        }

        internal async Task UpdateExpense(Expense myExpense, SplitExpenseVM viewModel)
        {
            if (viewModel.FriendIds != "") 
            {
                try
                {
                    var newPayers = viewModel.FriendIds.Split(',');

                    var payerIds = new List<int>();

                    foreach (var payer in newPayers)
                    {
                        payerIds.Add(Convert.ToInt32(payer));  
                    }

                    var oldPayers = PayersForExpense.Where(p => p.ExpenseId == myExpense.Id).ToList();

                    //Radera de oldpayers som inte är med i payerIds
                    foreach (var payer in oldPayers)
                    {
                        if (payerIds.Where(p => p == payer.UserId).Count() == 0)
                            PayersForExpense.Remove(payer);
                    }

                    //Lägg till de payerIds som inte fanns i oldPayers.
                    foreach (var payer in payerIds)
                    {
                        if (oldPayers.Where(p => p.UserId == payer).Count() == 0)
                            PayersForExpense.Add(new PayersForExpense { UserId = payer, ExpenseId = myExpense.Id });
                    }

                    SaveChanges();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            var myEvent = GetEventById(Convert.ToInt32(viewModel.SelectedEvent));
            myExpense.Amount = Convert.ToDecimal(viewModel.Amount);
            myExpense.Description = viewModel.Description;
            myExpense.CurrencyId = Convert.ToInt32(viewModel.SelectedCurrency);
            myExpense.Date = Convert.ToDateTime(viewModel.Date);
            myExpense.EventId = myEvent.Id;
            myExpense.AmountInStandardCurrency = await CalculateStandardCurrencyAmount(Convert.ToDecimal(viewModel.Amount),viewModel.SelectedCurrency, myEvent.StandardCurrencyId, Convert.ToDateTime(viewModel.Date));
            SaveChanges();
        }

        internal List<User> GetPayersByExpense(Expense expense)
        {
            var payers = new List<User>();

            foreach (var payer in expense.PayersForExpense)
            {
                payers.Add(payer.User);
            }

            return payers;
        }
    }
}
