using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class Assignment
	{
		public int ID { get; set; }
		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Beschreibung")]
		[Required]
		public string Description { get; set; }

		[Display(Name = "Diskussion")]
		[Required]
		public virtual Topic Topic { get; set; }

		[Display(Name = "Besitzer")]
		public virtual User Owner { get; set; }

		[Display(Name = "Deadline")]
		[Required]
		public DateTime DueDate { get; set; }

		[Display(Name = "Erinnert")]
		[Required]
		public bool ReminderSent { get; set; }

		[DisplayName("Erledigt")]
		public bool IsDone { get; set; }
	}
}