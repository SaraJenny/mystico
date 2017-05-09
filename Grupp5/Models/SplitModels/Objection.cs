using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    class Objection
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int OpponentID { get; set; }
        public int ExpenseID { get; set; }
    }
}
