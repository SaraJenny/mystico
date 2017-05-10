using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class PayersForExpense
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ExpenseId { get; set; }
        public string ObjectionDescription { get; set; }
        public bool Objection { get; set; }

        public virtual Expense Expense { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
