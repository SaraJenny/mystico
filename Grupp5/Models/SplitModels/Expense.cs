﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    class Expense
    {
        public int ID { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public int CurrencyID { get; set; }
        public User Purchaser { get; set; } //Change to PurchaserID?
        public int EventID { get; set; }
        public DateTime Date { get; set; }
        public double AmountInStandardCurrency { get; set; }

        public List<Objection> objections { get; set; }
        public List<User> payers { get; set; }

    }
}
