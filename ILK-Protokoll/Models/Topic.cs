using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Models
{
	public class Topic
	{
		public Topic()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Tags = new List<TagTopic>();
			Comments = new List<Comment>();
			Votes = new List<Vote>();
			Assignments = new List<Assignment>();
			Documents = new List<Document>();
			Created = DateTime.Now;
			ValidFrom = DateTime.Now;
			UnreadBy = new List<UnreadState>();
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


		[Display(Name = "Wiedervorlagedatum")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? ResubmissionDate { get; set; }

		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Tags")]
		public virtual ICollection<TagTopic> Tags { get; set; }

		[Display(Name = "Uhrzeit")]
		[Required(AllowEmptyStrings = true)]
		public string Time { get; set; }

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

		[Display(Name = "Zur Kenntnisnahme pushen an")]
		public virtual ICollection<PushNotification> PushTargets { get; set; }

		[Display(Name = "Aufgaben")]
		public virtual ICollection<Assignment> Assignments { get; set; }

		[Display(Name = "Entscheidung")]
		public virtual Decision Decision { get; set; }

		[Display(Name = "Priorität")]
		[Required]
		public Priority Priority { get; set; }

		[Display(Name = "Dokumente")]
		public virtual ICollection<Document> Documents { get; set; }

		[Display(Name = "Erstellt")]
		[Required]
		public DateTime Created { get; set; }

		[Display(Name = "Ersteller")]
		[ForeignKey("CreatorID")]
		public virtual User Creator { get; set; }

		[Required]
		public int CreatorID { get; set; }

		/// <summary>
		///    Gibt das Datum der letzten Änderung an. Falls der Diskussionspunkt nie geändert wurde, gleicht das Änderungsdatum
		///    der Erstelldatum.
		/// </summary>
		[Display(Name = "Geändert")]
		[Required]
		public DateTime ValidFrom { get; set; }

		[Display(Name = "Schreibschutz")]
		public bool IsReadOnly { get; set; }

		[NotMapped] // Muss bei Bedarf durch den Controller gesetzt werden
		public bool IsLocked { get; set; }

		public TopicLock Lock { get; set; }

		public virtual ICollection<UnreadState> UnreadBy { get; set; }

		public bool IsUnreadBy(int userID)
		{
			return UnreadBy.Any(x => x.UserID == userID);
		}

		public bool HasDecision()
		{
			return Decision != null;
		}

		public bool HasDecision(DecisionType d)
		{
			return Decision != null && Decision.Type == d;
		}

		internal AuthResult IsEditableBy(User u, ActiveSession s)
		{
			if (IsReadOnly)
				return new AuthResult("Dieser Diskussionspunkt ist nicht bearbeitbar.");
			else if (Lock != null)
			{
				if (!u.Equals(Lock.Session.Manager))
					return new AuthResult("Dieser Diskussionspunkt ist gesperrt, und nur durch den Sitzungsleiter bearbeitbar.");
			}
			else
			{
				if (u.Equals(Owner))
					return new AuthResult(true);

				if (s == null)
					return new AuthResult("Sie können diesen Diskussionspunkt nicht bearbeiten, da sie nicht der Besitzer sind.");
				else if (s.SessionType.ID != SessionTypeID)
				{
					return
						new AuthResult("Sie können diesen Diskussionspunkt nicht bearbeiten, da der Punkt nicht in ihre Sitzung fällt.");
				}
			}
			return new AuthResult(true);
		}

		public void IncorporateUpdates(TopicEdit updates)
		{
			if (IsReadOnly)
				throw new InvalidOperationException("Diese Diskussion ist beendet und kann daher nicht bearbeitet werden.");

			Description = updates.Description;
			OwnerID = updates.OwnerID;
			Priority = updates.Priority;
			Proposal = updates.Proposal;
			ResubmissionDate = updates.ResubmissionDate;
			Title = updates.Title;
			Time = updates.Time;
			ValidFrom = DateTime.Now;
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

	internal struct AuthResult
	{
		public readonly bool IsAuthorized;
		public readonly string Reason;

		public AuthResult(string reason)
		{
			IsAuthorized = false;
			Reason = reason;
		}

		public AuthResult(bool isAuthorized)
		{
			IsAuthorized = isAuthorized;
			Reason = "";
		}
	}

	[Serializable]
	public sealed class TopicLockedException : Exception
	{
		public TopicLockedException()
			: base("Das Thema ist gesperrt und kann daher nur durch den Sitzungsleiter bearbeitet werden.")
		{
		}

		public TopicLockedException(string message)
			: base(message)
		{
		}
	}
}