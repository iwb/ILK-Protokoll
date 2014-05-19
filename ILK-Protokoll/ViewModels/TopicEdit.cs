using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class TopicEdit
	{
		public TopicEdit()
		{
			AuditorList = new List<User>();
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			ToDo = new List<ToDo>();
			Duties = new List<Duty>();
			Attachments = new List<Attachment>();
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		[Display(Name = "DiPuN")]
		public int ID { get; set; }

		public User Owner { get; set; }

		[Display(Name = "Besitzer")]
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

		public SelectList UserList { get; set; }
		public SelectList SessionTypeList { get; set; }
		public SelectList TargetSessionTypeList { get; set; }

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


		[Display(Name = "ToDo")]
		public virtual ICollection<ToDo> ToDo { get; set; }

		[Display(Name = "Umsetzungsaufgaben")]
		public virtual ICollection<Duty> Duties { get; set; }

		[Display(Name = "Priorität")]
		[Required]
		public Priority Priority { get; set; }

		[Display(Name = "Dateianhänge")]
		public virtual ICollection<Attachment> Attachments { get; set; }

		public static TopicEdit FromTopic(Topic t)
		{
			return new TopicEdit
			{
				Attachments = t.Attachments,
				AuditorList = t.AuditorList,
				Description = t.Description,
				Duties = t.Duties,
				ID = t.ID,
				Owner = t.Owner,
				Priority = t.Priority,
				Proposal = t.Proposal,
				SessionType = t.SessionType,
				SessionTypeID = t.SessionTypeID,
				TargetSessionType = t.TargetSessionType,
				TargetSessionTypeID = t.TargetSessionTypeID,
				Title = t.Title,
				ToDo = t.ToDo
			};
		}
	}
}