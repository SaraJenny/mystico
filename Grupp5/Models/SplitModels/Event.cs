using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    class Event
    {
        public int ID { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int StandardCurrencyID { get; set; } //Kopplas till Valuta-klassen

        public List<Person> participants { get; set; }
        public List<Expense> expenses { get; set; }

        //TODO metod för att sätta TotalCostPerEvent

    }
}
