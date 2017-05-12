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
		[Required(ErrorMessage = "Namn saknas")]
		[Display(Name = "Namn")]
		public string Name { get; set; }

		[Display(Name = "Beskrivning")]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		public SelectListItem[] CurrencyItem { get; set; }

		[Required(ErrorMessage = "Standardvaluta saknas")]
		[Display(Name = "Standardvaluta")]
		public string SelectedCurrency { get; set; }

		public string FriendIds { get; set; }
	}
}
