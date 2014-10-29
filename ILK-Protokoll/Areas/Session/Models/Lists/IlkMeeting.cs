using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    ILK-Regeltermine
	/// </summary>
	[Table("L_IlkMeeting")]
	public class IlkMeeting : BaseItem
	{
		public IlkMeeting()
		{
			Start = DateTime.Today.AddHours(9);
		}

		[DisplayName("Beginn")]
		[DataType(DataType.DateTime)]
		public DateTime Start { get; set; }


		[Required]
		[DisplayName("Ort")]
		public string Place { get; set; }

		[DisplayName("Sitzungstyp")]
		public virtual SessionType SessionType { get; set; }

		[ForeignKey("SessionType")]
		[DisplayName("Sitzungstyp")]
		public int SessionTypeID { get; set; }

		[DisplayName("Verant.")]
		public virtual User Organizer { get; set; }

		[ForeignKey("Organizer")]
		public int OrganizerID { get; set; }

		[Required(AllowEmptyStrings = true)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		[DisplayName("Anmerkung")]
		public string Comments { get; set; }
	}
}