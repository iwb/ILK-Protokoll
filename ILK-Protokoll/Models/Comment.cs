using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Comment
	{
		public Comment()
		{
			Created = DateTime.Now;
		}
		public int ID { get; set; }

		public User Author { get; set; }

		public DateTime Created { get; set; }

		public string Content { get; set; }

	}
}