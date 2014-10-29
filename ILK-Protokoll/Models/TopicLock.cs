using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Session.Models;

namespace ILK_Protokoll.Models
{
	public class TopicLock
	{
		[Key]
		public int TopicID { get; set; }

		[Required]
		public virtual ActiveSession Session { get; set; }

		[Required]
		public virtual Topic Topic { get; set; }

		[Required]
		public TopicAction Action { get; set; }

		[NotMapped]
		public string Message { get; set; }
	}

	public enum TopicAction
	{
		[Display(Name = "Wiedervorlage")] None,
		[Display(Name = "Beschluss")] Decide,
		[Display(Name = "Archiviert")] Close,
		[Display(Name = "Löschen")] Delete
	}
}