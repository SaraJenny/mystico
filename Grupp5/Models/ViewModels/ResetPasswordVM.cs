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

        [Required(ErrorMessage = "Lösenord saknas")]
        [Display(Name = "Lösenord")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Minst 5 tecken krävs")]
        public string Password { get; set; }

		[Required(ErrorMessage = "Lösenord saknas")]
		[DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
		[Compare("Password", ErrorMessage = "Lösenorden överensstämmer inte")]
		public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
