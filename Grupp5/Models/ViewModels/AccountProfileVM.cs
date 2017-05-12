using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class AccountProfileVM
    {
        [Required(ErrorMessage = "Förnamn saknas")]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Efternamn saknas")]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-post saknas")]
        [Display(Name = "E-post")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Nuvarande lösenord")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "Lösenord")]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Minst 5 tecken krävs")]
        public string Password { get; set; }

        [Display(Name = "Upprepa lösenord")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenorden överensstämmer inte")]
        public string PasswordCheck { get; set; }

    }
}
