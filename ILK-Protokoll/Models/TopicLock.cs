using System.ComponentModel.DataAnnotations;
using ILK_Protokoll.Areas.Session.Models;

namespace ILK_Protokoll.Models
{
	public class TopicLock
	{
		[Key]
		public int TopicID { get; set; }

		[Required]
		public ActiveSession Session { get; set; }

		[Required]
		public Topic Topic { get; set; }

		[Required]
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