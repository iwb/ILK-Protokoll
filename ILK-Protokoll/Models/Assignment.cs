using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class Assignment
	{
		public Assignment()
		{
			DueDate = DateTime.Today;
		}

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
		public int TopicID { get; set; }

		[ForeignKey("TopicID")]
		public virtual Topic Topic { get; set; }

		[Display(Name = "Besitzer")]
		public int OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public virtual User Owner { get; set; }

		[Display(Name = "Deadline")]
		[DataType(DataType.Date)]
		[Required]
		public DateTime DueDate { get; set; }

		[Column("Active")]
		[Display(Name = "Aktiv")]
		public bool IsActive { get; set; }

		[Display(Name = "Erinnert")]
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