
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitDetailsVM
    {
        public string expenseInformation { get; set; }
        public bool objectionExists { get; set; } = false;
        public string objectionMessage { get; set; }
    }
}
