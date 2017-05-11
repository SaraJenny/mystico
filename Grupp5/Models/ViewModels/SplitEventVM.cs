using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitEventVM
    {
		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Namn")]
		public string Name { get; set; }

		[Display(Name = "Beskrivning")]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		public SelectListItem[] CurrencyItem { get; set; }

		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Standardvaluta")]
		public string SelectedCurrency { get; set; }

		// TODO Lägg till vänner efter namn
		public string FriendIds { get; set; }
	}
}
