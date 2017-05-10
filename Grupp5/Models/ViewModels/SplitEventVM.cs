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

//		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Beskrivning")]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		// TODO Gör detta till en select
		[Required(ErrorMessage = "Obligatoriskt fält")]
		[Display(Name = "Standardvaluta")]
		public string Currency { get; set; }

		// TODO Lägg till vänner
	}
}
