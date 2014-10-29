using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class UnreadState : IEquatable<UnreadState>
	{
		public UnreadState()
		{
			LatestChange = DateTime.Now;
		}

		public int ID { get; set; }

		public int TopicID { get; set; }

		[ForeignKey("TopicID")]
		public virtual Topic Topic { get; set; }

		public int UserID { get; set; }

		[ForeignKey("UserID")]
		public virtual User User { get; set; }

		public DateTime LatestChange { get; set; }

		public bool Equals(UnreadState other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && Equals((UnreadState)obj);
		}

		public override int GetHashCode()
		{
			return ID;
		}
	}
}