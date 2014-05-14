using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{

	public class Topic
	{
		public int ID { get; set; }

		[Display(Name = "Besitzer")]
		[Required]
		public string Owner { get; set; }

		[Display(Name = "Prüfer")]
		[Required]
		public List<string> AuditorList { get; set; }

		[Display(Name = "Zugeordneter Sitzungstyp")]
		[Required]
		public virtual SessionType SessionType { get; set; }

		[Display(Name = "Zukünftiger Sitzungstyp")] // Falls der DP gerade verschoben wird
		public virtual SessionType TargetSessionType { get; set; }

		[Display(Name = "Sitzungstyp")]
		[ForeignKey("SessionType")]
		public virtual int SessionTypeID { get; set; }

		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Beschreibung")]
		[Required]
		public string Description { get; set; }

		[Display(Name = "Beschlussvorschlag")]
		[Required]
		public string Proposal { get; set; }

		[Display(Name = "Kommentare")]
		public virtual List<Comment> Comments { get; set; }

		[Display(Name = "ToDos")]
		public virtual List<ToDo> ToDos { get; set; }

		[Display(Name = "Umsetzungsaufgaben")]
		public virtual List<Duty> Duties { get; set; }

		[Display(Name = "Beschluss")]
		public virtual Decision Decision { get; set; }

		[Display(Name = "Priorität")]
		[Required]
		public Priority Priority { get; set; }

		[Display(Name = "Dateianhänge")]
		public virtual List<Attachment> Attachments { get; set; }

		[Display(Name = "Erstellt")]
		[Required]
		public DateTime Created { get; set; }

		[Display(Name = "Geändert")]
		[Required]
		public DateTime LastChanged { get; set; }
	}
}