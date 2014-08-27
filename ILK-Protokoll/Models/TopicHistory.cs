using System;
using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Models
{
	public class TopicHistory
	{
		[Required]
		public int ID { get; set; }

		[Required]
		public int TopicID { get; set; }

		[Required]
		public int EditorID { get; set; }

		[Required]
		public string Title { get; set; }

		[Required(AllowEmptyStrings = true)]
		public string Time { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string Proposal { get; set; }

		[Required]
		public int OwnerID { get; set; }

		[Required]
		public int SessionTypeID { get; set; }

		public int? TargetSessionTypeID { get; set; }

		[Required]
		public Priority Priority { get; set; }

		[Required]
		public DateTime ValidFrom { get; set; }

		[Required]
		public DateTime ValidUntil { get; set; }

		public static TopicHistory FromTopic(Topic t, int editorID)
		{
			return new TopicHistory
			{
				TopicID = t.ID,
				EditorID = editorID,
				Title = t.Title,
				Time = t.Time,
				Description = t.Description,
				Proposal = t.Proposal,
				OwnerID = t.OwnerID,
				SessionTypeID = t.SessionTypeID,
				TargetSessionTypeID = t.TargetSessionTypeID,
				Priority = t.Priority,
				ValidFrom = t.ValidFrom,
				ValidUntil = DateTime.Now
			};
		}
	}
}