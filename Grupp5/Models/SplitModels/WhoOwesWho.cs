using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class WhoOwesWho
    {
        public User Debitor { get; set; }

        public User Creditor { get; set; }

        public decimal Amount { get; set; }
    }
}
