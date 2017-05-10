using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class User
    {
        public string AspId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Id { get; set; }

        public virtual AspNetUsers Asp { get; set; }
    }
}
