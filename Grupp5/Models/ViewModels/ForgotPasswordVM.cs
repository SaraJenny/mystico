using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class ForgotPasswordVM
    {
		[Required(ErrorMessage = "E-post saknas")]
		[Display(Name = "Fyll i din e-post")]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
    }
}
