using Grupp5.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public partial class MysticoContext: DbContext
    {
        public MysticoContext(DbContextOptions<MysticoContext> options) : base (options)
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
    }
}
