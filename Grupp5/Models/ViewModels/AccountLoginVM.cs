using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class AccountLoginVM
    {
		[Required(ErrorMessage = "E-post saknas")]
		[Display(Name = "E-post")]
		public string Email { get; set; }


		[Required(ErrorMessage = "Lösenord saknas")]
		[Display(Name = "Lösenord")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
