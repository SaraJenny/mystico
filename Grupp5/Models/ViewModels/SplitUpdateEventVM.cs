﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitUpdateEventVM
    {
		public int EventId { get; set; }

		[Required(ErrorMessage = "Namn saknas")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public SelectListItem[] CurrencyItem { get; set; }

		[Required(ErrorMessage = "Återbetalningsvaluta saknas")]
		[Display(Name = "Återbetalningsvaluta")]
		public string SelectedCurrency { get; set; }

		[Required(ErrorMessage = "Utgiftsvaluta saknas")]
		[Display(Name = "Utgiftsvaluta")]
		public string ExpenseCurrencyId { get; set; }

		public List<UserVM> AlreadyParticipants { get; set; }

        public string FriendIds { get; set; }
    }
}
