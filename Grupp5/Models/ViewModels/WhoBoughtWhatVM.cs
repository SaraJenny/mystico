using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class WhoBoughtWhatVM
    {
        public int PurchaserId { get; set; }
        public string PurchaserFirstName { get; set; }
        public string PurchaserLastName { get; set; }
        public string PurchaserEmail { get; set; }
        public int Amount { get; set; }
        public List<PayerVM> payers { get; set; }
        public string ExpenseDescription { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Date { get; set; }


    }

}
