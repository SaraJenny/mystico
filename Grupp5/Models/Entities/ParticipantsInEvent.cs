using System;
using System.Collections.Generic;

namespace Grupp5.Models.Entities
{
    public partial class ParticipantsInEvent
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }

        public virtual Event Event { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
