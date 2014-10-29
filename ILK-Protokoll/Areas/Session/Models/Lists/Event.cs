using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Termine und Veranstaltungen, die das ganze Institut betreffen
	/// </summary>
	[Table("L_Event")]
	public class Event : BaseItem
	{
		public Event()
		{
			StartDate = DateTime.Today;
			EndDate = DateTime.Today;
		}

		[DisplayName("Von")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime StartDate { get; set; }

		[DisplayName("Bis")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime EndDate { get; set; }

		[Required(AllowEmptyStrings = true)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		[DisplayName("Uhrzeit")]
		public string Time { get; set; }

		[Required(AllowEmptyStrings = true)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		[DisplayName("Ort")]
		public string Place { get; set; }

		[Required]
		[DisplayName("Verant.")]
		public string Organizer { get; set; }

		[Required]
		[DisplayName("Besucher / Thema")]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
	}
}