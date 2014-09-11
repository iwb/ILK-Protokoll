using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		[ForeignKey("Author")]
		public virtual int? AuthorID { get; set; }

		public DateTime Created { get; set; }

		[Required]
		public string Content { get; set; }
	}
}