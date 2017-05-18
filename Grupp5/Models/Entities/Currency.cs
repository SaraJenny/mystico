using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class Currency
    {
        public Currency()
        {
            EventExpenseCurrency = new HashSet<Event>();
            EventStandardCurrency = new HashSet<Event>();
            Expense = new HashSet<Expense>();
        }

        public int Id { get; set; }
        public string CurrencyCode { get; set; }

        public virtual ICollection<Event> EventExpenseCurrency { get; set; }
        public virtual ICollection<Event> EventStandardCurrency { get; set; }
        public virtual ICollection<Expense> Expense { get; set; }
    }
}
