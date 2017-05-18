using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class Event
    {
        public Event()
        {
            Expense = new HashSet<Expense>();
            ParticipantsInEvent = new HashSet<ParticipantsInEvent>();
        }

        public int Id { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int StandardCurrencyId { get; set; }
        public int ExpenseCurrencyId { get; set; }

        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<ParticipantsInEvent> ParticipantsInEvent { get; set; }
        public virtual Currency ExpenseCurrency { get; set; }
        public virtual Currency StandardCurrency { get; set; }
    }
}
