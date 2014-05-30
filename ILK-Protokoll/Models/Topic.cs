﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Models
{
	public class Topic
	{
		public Topic()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Comments = new List<Comment>();
			Votes = new List<Vote>();
			ToDo = new List<ToDo>();
			Duties = new List<Duty>();
			Attachments = new List<Attachment>();
			Created = DateTime.Now;
			ValidFrom = DateTime.Now;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public void IncorporateUpdates(TopicEdit updates)
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Attachments = updates.Attachments;
			Comments = new List<Comment>();
			Description = updates.Description;
			Duties = updates.Duties;
			Owner = updates.Owner;
			OwnerID = updates.OwnerID;
			Priority = updates.Priority;
			Proposal = updates.Proposal;
			Title = updates.Title;
			ToDo = updates.ToDo;
			ValidFrom = DateTime.Now;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public void IncorporateHistory(TopicHistory history)
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Description = history.Description;
			OwnerID = history.OwnerID;
			Proposal = history.Proposal;
			SessionTypeID = history.SessionTypeID;
			Title = history.Title;
			ValidFrom = history.ValidFrom;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		[Display(Name = "DiPuN")]
		public int ID { get; set; }

		[Display(Name = "Besitzer")]
		[Required]
		public int OwnerID { get; set; }

		[ForeignKey("OwnerID")]
		public virtual User Owner { get; set; }

		public virtual SessionType SessionType { get; set; }

		[Display(Name = "Sitzungstyp")]
		[Required]
		public int SessionTypeID { get; set; }

		public virtual SessionType TargetSessionType { get; set; }

		[Display(Name = "Zukünftiger Sitzungstyp")] // Falls der DP gerade verschoben wird
		public int? TargetSessionTypeID { get; set; }

		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Beschreibung")]
		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		[Display(Name = "Beschlussvorschlag")]
		[DataType(DataType.MultilineText)]
		[Required]
		public string Proposal { get; set; }

		[Display(Name = "Kommentare")]
		public virtual ICollection<Comment> Comments { get; set; }

		[Display(Name = "Stimmen")]
		public virtual ICollection<Vote> Votes { get; set; }

		[Display(Name = "ToDo")]
		public virtual ICollection<ToDo> ToDo { get; set; }

		[Display(Name = "Umsetzungsaufgaben")]
		public virtual ICollection<Duty> Duties { get; set; }

		[Display(Name = "Beschluss")]
		public virtual Decision Decision { get; set; }

		[Display(Name = "Priorität")]
		[Required]
		public Priority Priority { get; set; }

		[Display(Name = "Dateianhänge")]
		public virtual ICollection<Attachment> Attachments { get; set; }

		[Display(Name = "PDF-Report")]
		public virtual Attachment Report { get; set; }

		[Display(Name = "Erstellt")]
		[Required]
		public DateTime Created { get; set; }

		/// <summary>
		///    Jeder Diskussionspunkt kann mehrere Einträge in der Datenbank haben.
		///    Es ist nur derjenige gültig, der hier das spätestes Datum besitzt.
		///    Der Rest sind archivierte Versionen desselben Diskussionspunkts.
		/// </summary>
		[Display(Name = "Geändert")]
		[Required]
		public DateTime ValidFrom { get; set; }
	}
}