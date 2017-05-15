using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitOverviewVM
    {
        public bool EventIsActive { get; set; }
        public string EventName { get; set; }
		public int MyStatus { get; set; }
		public int Total { get; set; }
		public int MyTotal { get; set; }
		public List<WhoOwesWhoVM> TransactionsCommittedToMe  { get; set; }
		public List<WhoOwesWhoVM> TransactionsWithoutMe { get; set; }
		public string FriendIds { get; set; }
        public string Message { get; set; }
        public int EventId { get; set; }
        public string StandardCurrency { get; set; }
        public List<UserVM> AlreadyParticipants { get; set; }

    }
}
