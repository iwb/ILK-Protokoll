using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

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

		[ForeignKey("Voter")]
		public int? VoterID { get; set; }

		public virtual Topic Topic { get; set; }

		[ForeignKey("Topic")]
		public int? TopicID { get; set; }

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