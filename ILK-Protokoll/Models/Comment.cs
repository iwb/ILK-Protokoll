using System;
using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Models
{
	public class Comment
	{
		public Comment()
		{
			Created = DateTime.Now;
		}

		public int ID { get; set; }

		public int TopicID { get; set; }

		public virtual User Author { get; set; }

		public DateTime Created { get; set; }

		[Required]
		public string Content { get; set; }
	}
}