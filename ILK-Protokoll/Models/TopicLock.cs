using System;
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
		public ActiveSession Session { get; set; }

		[Required]
		public Topic Topic { get; set; }

		[Required]
		public TopicAction Action { get; set; }

		[NotMapped]
		public string Message { get; set; }


		[Display(Name = "Wiedervorlagedatum")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? ResubmissionDate { get; set; }
	}

	public enum TopicAction
	{
		[Display(Name = "Wiedervorlage")]
		None,
		[Display(Name = "Beschluss")]
		Decide,
		[Display(Name = "Archiviert")]
		Close,
		[Display(Name = "Löschen")]
		Delete
	}
}