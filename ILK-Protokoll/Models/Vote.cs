using System.ComponentModel.DataAnnotations;
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
		[Display(Name = "Neutral", Order = 1)] None,
		[Display(Name = "Abgelehnt", Order = 4)] Rejected,
		[Display(Name = "Gespächsbedarf", Order = 2)] Reservation,
		[Display(Name = "Angenommen", Order = 3)] Approved
	}
}