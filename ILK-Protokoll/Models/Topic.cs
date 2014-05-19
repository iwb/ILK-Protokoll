using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ILK_Protokoll.ViewModels;

namespace ILK_Protokoll.Models
{
	public class Topic
	{
		[NotMapped] private List<Vote> _displayvotes;

		public Topic()
		{
// ReSharper disable DoNotCallOverridableMethodsInConstructor
			AuditorList = new List<User>();
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
			AuditorList = new List<User>(updates.AuditorList);
			Comments = new List<Comment>();
			Description = updates.Description;
			Duties = updates.Duties;
			Owner = updates.Owner;
			Priority = updates.Priority;
			Proposal = updates.Proposal;
			SessionType = updates.SessionType;
			SessionTypeID = updates.SessionTypeID;
			TargetSessionType = updates.TargetSessionType;
			TargetSessionTypeID = updates.TargetSessionTypeID;
			Title = updates.Title;
			ToDo = updates.ToDo;
			ValidFrom = DateTime.Now;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		[Display(Name = "DiPuN")]
		public int ID { get; set; }

		public User Owner { get; set; }

		[Display(Name = "Besitzer")]
		[ForeignKey("Owner")]
		[Required]
		public int OwnerID { get; set; }

		[Display(Name = "Prüfer")]
		[Required]
		public ICollection<User> AuditorList { get; set; }

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

		public ICollection<Vote> GetDisplayVotes(User currentUser)
		{
			if (_displayvotes == null)
			{
				_displayvotes = new List<Vote>();
				_displayvotes.Add(LookupVote(currentUser));

				foreach (
					User person in
						AuditorList.Except(new[] {currentUser}).OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase))
					_displayvotes.Add(LookupVote(person));
			}

			return _displayvotes;
		}

		private Vote LookupVote(User u)
		{
			Vote voter = Votes.SingleOrDefault(x => x.Voter == u);
			return new Vote(u, voter != null ? voter.Kind : VoteKind.None);
		}
	}
}