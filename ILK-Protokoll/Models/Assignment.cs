using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Models
{
	public class Assignment
	{
		public int ID { get; set; }

		[Display(Name = "Typ")]
		public AssignmentType Type { get; set; }

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

	public enum AssignmentType
	{
		[Display(Name = "ToDo")] ToDo,
		[Display(Name = "Umsetzung")] Duty
	}
}