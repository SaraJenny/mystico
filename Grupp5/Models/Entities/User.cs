using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class User
    {
        public User()
        {
            Expense = new HashSet<Expense>();
            ParticipantsInEvent = new HashSet<ParticipantsInEvent>();
            PayersForExpense = new HashSet<PayersForExpense>();
        }

        public string AspId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<ParticipantsInEvent> ParticipantsInEvent { get; set; }
        public virtual ICollection<PayersForExpense> PayersForExpense { get; set; }
    }
}
