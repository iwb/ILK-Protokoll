using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Administration.Models;
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
			Assignments = new List<Assignment>();
			Attachments = new List<Attachment>();
			Created = DateTime.Now;
			ValidFrom = DateTime.Now;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		[Display(Name = "Topic-ID")]
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

		[Display(Name = "Aufgaben")]
		public virtual ICollection<Assignment> Assignments { get; set; }

		[Display(Name = "Beschluss")]
		public virtual Decision Decision { get; set; }

		[Display(Name = "Priorität")]
		[Required]
		public Priority Priority { get; set; }

		[Display(Name = "Dateianhänge")]
		public virtual ICollection<Attachment> Attachments { get; set; }

		[Display(Name = "Erstellt")]
		[Required]
		public DateTime Created { get; set; }

		[Display(Name = "Ersteller")]
		[Required]
		public User Creator { get; set; }

		/// <summary>
		///    Gibt das Datum der letzten Änderung an. Falls der Diskussionspunkt nie geändert wurde, gleicht das Änderungsdatum
		///    der Erstelldatum.
		/// </summary>
		[Display(Name = "Geändert")]
		[Required]
		public DateTime ValidFrom { get; set; }

		[NotMapped]
		public bool IsEditable
		{
			get { return Decision == null; }
		}

		public void IncorporateUpdates(TopicEdit updates)
		{
			if (!IsEditable)
				throw new InvalidOperationException("Diese Diskussion ist beendet und kann daher nicht bearbeitet werden.");

			Description = updates.Description;
			Owner = updates.Owner;
			OwnerID = updates.OwnerID;
			Priority = updates.Priority;
			Proposal = updates.Proposal;
			Title = updates.Title;
			ValidFrom = DateTime.Now;
		}

		public void IncorporateHistory(TopicHistory history)
		{
			if (!IsEditable)
				throw new InvalidOperationException("Diese Diskussion ist beendet und kann daher nicht bearbeitet werden.");
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Description = history.Description;
			OwnerID = history.OwnerID;
			Proposal = history.Proposal;
			SessionTypeID = history.SessionTypeID;
			Priority = history.Priority;
			Title = history.Title;
			ValidFrom = history.ValidFrom;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}
	}

	internal class TopicByIdComparer : IEqualityComparer<Topic>
	{
		public bool Equals(Topic x, Topic y)
		{
			return x.ID == y.ID;
		}

		public int GetHashCode(Topic obj)
		{
			return obj.ID;
		}
	}
}