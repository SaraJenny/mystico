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

		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Event")]
		public string SelectedEvent { get; set; }

		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Beskrivning")]
		public string Description { get; set; }

		public SelectListItem[] CurrencyItem { get; set; }

		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Valuta")]
		public string SelectedCurrency { get; set; }

		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Belopp")]
		[Range(0.0, double.MaxValue, ErrorMessage = "Ogiltigt belopp")]
		public string Amount { get; set; }

		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Datum")]
		[DataType(DataType.Date)]
		public string Date { get; set; }

        public List<UserVM> Payers { get; set; }
    }
}
