using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class WhoOwesWhoVM
    {
        public int DebitorId { get; set; }
        public string DebitorFirstName { get; set; }
        public string DebitorLastName { get; set; }
        public string DebitorEmail { get; set; }

        public int CreditorId { get; set; }
        public string CreditorFirstName { get; set; }
        public string CreditorLastName { get; set; }
        public string CreditorEmail { get; set; }

        public int Amount { get; set; }
    }
}
