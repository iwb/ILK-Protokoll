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
		[DisplayName("Beginn")]
		[DataType(DataType.DateTime)]
		public DateTime Start { get; set; }


		[DisplayName("Ort")]
		public string Place { get; set; }


		[DisplayName("Sitzungstyp")]
		public virtual SessionType SessionType { get; set; }

		[DisplayName("Verant.")]
		public virtual User Organizer { get; set; }

		[DisplayName("Anmerkung")]
		public string Comments { get; set; }
	}
}