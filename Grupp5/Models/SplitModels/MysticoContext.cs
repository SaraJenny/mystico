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

        public void AddUser(string id, string firstName, string lastName)
        {
            try
            {
                User.Add(new User
                {
                    AspId = id,
                    FirstName = firstName,
                    LastName = lastName
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

        public void AddParticipantsToEvent(List<int> friends, int eventId, int currentUserId)
        {
            ParticipantsInEvent.Add(new ParticipantsInEvent
            {
                EventId = eventId,
                UserId = currentUserId,
            });

            foreach (var userId in friends)
            {
                if (userId != currentUserId)
                {
                    ParticipantsInEvent.Add(new ParticipantsInEvent
                    {
                        EventId = eventId,
                        UserId = userId,
                    });
                }
            };

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

        internal void CreatePayerForExpense(List<UserVM> payers, int expenseId)
        {
            foreach (var payer in payers)
            {
                if (payer.IsSelected)
                {
                    PayersForExpense.Add(new PayersForExpense
                    {

                        ExpenseId = expenseId,
                        UserId = payer.Id,
                        Objection = false

                    });
                }
            }

            SaveChanges();
        }
    }
}
