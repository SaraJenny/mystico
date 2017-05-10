using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class Expense
    {
        public Expense()
        {
            PayersForExpense = new HashSet<PayersForExpense>();
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int CurrencyId { get; set; }
        public string PurchaserId { get; set; }
        public int EventId { get; set; }
        public DateTime Date { get; set; }
        public decimal AmountInStandardCurrency { get; set; }

        public virtual ICollection<PayersForExpense> PayersForExpense { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Event Event { get; set; }
        public virtual AspNetUsers Purchaser { get; set; }
    }
}
