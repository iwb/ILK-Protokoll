using System;

namespace ILK_Protokoll.Models
{
	public class TopicHistory
	{
		public int ID { get; set; }
		public int TopicID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Proposal { get; set; }
		public int OwnerID { get; set; }
		public int SessionTypeID { get; set; }

		public DateTime ValidFrom { get; set; }
		public DateTime ValidUntil { get; set; }

		public static TopicHistory FromTopic(Topic t)
		{
			return new TopicHistory
			{
				TopicID = t.ID,
				Title = t.Title,
				Description = t.Description,
				Proposal = t.Proposal,
				OwnerID = t.OwnerID,
				SessionTypeID = t.SessionTypeID,
				ValidFrom = t.ValidFrom,
				ValidUntil = DateTime.Now
			};
		}
	}
}