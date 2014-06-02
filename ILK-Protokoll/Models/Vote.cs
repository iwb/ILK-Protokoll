using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Vote
	{
		public Vote()
		{

		}

		public Vote(User voter, VoteKind vote)
		{
			Voter = voter;
			Kind = vote;
		}

		public int ID { get; set; }
		public virtual User Voter { get; set; }
		public virtual Topic Topic { get; set; }
		public VoteKind Kind { get; set; }
	}

	public enum VoteKind
	{
		[Description("Keine Stimme")]
		None,
		[Description("Abgelehnt")]
		Rejected,
		[Description("Gespächsbedarf")]
		Reservation,
		[Description("Angenommen")]
		Approved
	}
}