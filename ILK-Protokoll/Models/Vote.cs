using System.ComponentModel;

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
		[Description("Neutral")] None,
		[Description("Abgelehnt")] Rejected,
		[Description("Gespächsbedarf")] Reservation,
		[Description("Angenommen")] Approved
	}
}