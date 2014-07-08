using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Klausur-Tage
	/// </summary>
	[Table("L_IlkDay")]
	public class IlkDay : BaseItem
	{
		[DisplayName("Beginn")]
		[DataType(DataType.DateTime)]
		public DateTime Start { get; set; }

		[DisplayName("Ende")]
		[DataType(DataType.DateTime)]
		public DateTime End { get; set; }

		public string Place { get; set; }

		public SessionType SessionType { get; set; }

		public virtual User Organizer { get; set; }

		[ForeignKey("Organizer")]
		public int OrganizerID { get; set; }

		public string Topics { get; set; }

		public string Participants { get; set; }
	}
}