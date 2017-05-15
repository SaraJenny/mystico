using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.Entities
{
    public class SplitOverviewVM
    {
		public string EventName { get; set; }
		public int MyStatus { get; set; }
		public int Total { get; set; }
		public int MyTotal { get; set; }
		public List<WhoOwesWhoVM> TransactionsCommittedToMe  { get; set; }
		public List<WhoOwesWhoVM> TransactionsWithoutMe { get; set; }
		public string FriendIds { get; set; }
	}
}
