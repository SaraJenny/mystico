using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class ParticipantsInEvent
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }

        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
    }
}
