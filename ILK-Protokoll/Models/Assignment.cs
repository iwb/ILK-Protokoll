using System;

namespace ILK_Protokoll.Models
{
	public class Assignment
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public virtual Topic Topic { get; set; }
		public virtual User Owner { get; set; }
		public DateTime DueDate { get; set; }
		public bool ReminderSent { get; set; }
	}
}