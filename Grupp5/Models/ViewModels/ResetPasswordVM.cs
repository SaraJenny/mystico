using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class ResetPasswordVM
    {
		[Required(ErrorMessage = "E-post saknas")]
		[Display(Name = "E-post")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

        [Display(Name = "Lösenord")]
		[Required(ErrorMessage = "Lösenord saknas")]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "Lösenordet måste bestå av minst {2} tecken", MinimumLength = 6)]
        public string Password { get; set; }

		[Required(ErrorMessage = "Lösenord saknas")]
		[DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
		[Compare("Password", ErrorMessage = "Lösenorden överensstämmer inte")]
		public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
