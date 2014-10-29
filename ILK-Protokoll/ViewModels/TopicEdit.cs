using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class TopicEdit
	{
		public TopicEdit()
		{
			Priority = Priority.Medium;
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

		public SelectList UserList { get; set; }
		public SelectList SessionTypeList { get; set; }
		public SelectList TargetSessionTypeList { get; set; }

		[Display(Name = "Wiedervorlagedatum")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? ResubmissionDate { get; set; }

		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Uhrzeit")]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Time { get; set; }

		[Display(Name = "Beschreibung")]
		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		[Display(Name = "Beschlussvorschlag")]
		[DataType(DataType.MultilineText)]
		[Required]
		public string Proposal { get; set; }

		[Display(Name = "Priorität")]
		[Required]
		public Priority Priority { get; set; }

		public static TopicEdit FromTopic(Topic t)
		{
			return new TopicEdit
			{
				Description = t.Description,
				ID = t.ID,
				Owner = t.Owner,
				OwnerID = t.OwnerID,
				Priority = t.Priority,
				Proposal = t.Proposal,
				ResubmissionDate = t.ResubmissionDate,
				SessionType = t.SessionType,
				SessionTypeID = t.SessionTypeID,
				TargetSessionType = t.TargetSessionType,
				TargetSessionTypeID = t.TargetSessionTypeID,
				Title = t.Title,
				Time = t.Time,
			};
		}
	}
}