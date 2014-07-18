using System.ComponentModel.DataAnnotations;
using ILK_Protokoll.Areas.Session.Models;

namespace ILK_Protokoll.Models
{
	public class TopicLock
	{
		public int ID { get; set; }

		public ActiveSession Session { get; set; }

		[Required]
		public Topic Topic { get; set; }

		public TopicAction Action { get; set; }
	}

	public enum TopicAction
	{
		None,
		Decide,
		Close,
		Delete
	}
}