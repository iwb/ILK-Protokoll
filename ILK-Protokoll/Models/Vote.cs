using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Vote
	{
		public int ID { get; set; }
		public User Voter { get; set; }
		public VoteKind Kind { get; set; }
	}

	public enum VoteKind
	{
		None,
		Rejected,
		Reservation,
		Approved
	}
}