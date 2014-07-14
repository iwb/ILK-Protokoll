using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc.Html;

namespace ILK_Protokoll.Models
{
	public class TopicHistory
	{
		[Required]
		public int ID { get; set; }

		[Required]
		public int TopicID { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string Proposal { get; set; }

		[Required]
		public int OwnerID { get; set; }

		[Required]
		public int SessionTypeID { get; set; }

		[Required]
		public Priority Priority { get; set; }

		[Required]
		public DateTime ValidFrom { get; set; }

		[Required]
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
				Priority = t.Priority,
				ValidFrom = t.ValidFrom,
				ValidUntil = DateTime.Now
			};
		}
	}
}