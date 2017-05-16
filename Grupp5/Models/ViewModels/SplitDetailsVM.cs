
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitDetailsVM
    {
        public bool ObjectionExists { get; set; } = false;
        public List<string> ObjectionMessage { get; set; }
        public WhoBoughtWhatVM ExpenseInfo { get; set; }
        public int ExpenseId { get; set; }
    }
}
