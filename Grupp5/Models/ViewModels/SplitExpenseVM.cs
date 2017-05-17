using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitExpenseVM
    {
		public SelectListItem[] EventItem { get; set; }

		[Required(ErrorMessage = "Event saknas")]
		[Display(Name = "Event")]
		public int SelectedEvent { get; set; }

		[Required(ErrorMessage = "Beskrivning saknas")]
		[Display(Name = "Beskrivning")]
		public string Description { get; set; }

		public SelectListItem[] CurrencyItem { get; set; }

		[Required(ErrorMessage = "Valuta saknas")]
		[Display(Name = "Valuta")]
		public int SelectedCurrency { get; set; }

		[Required(ErrorMessage = "Belopp saknas")]
		[Display(Name = "Belopp")]
        [DataType(DataType.Currency)]
        [Range(0.0, double.MaxValue, ErrorMessage = "Ogiltigt belopp")]
        public string Amount { get; set; }

		[Required(ErrorMessage = "Betalningsdatum saknas")]
		[Display(Name = "Betalningsdatum")]
		[DataType(DataType.Date)]
        public string Date { get; set; }
        //public DateTime Date { get; set; }

        public List<UserVM> Payers { get; set; }

		public string FriendIds { get; set; }

        public int ExpenseId { get; set; }

        public int PurchaserID { get; set; }

        public SelectListItem[] PossiblePurchaser { get; set; }

    }
}
