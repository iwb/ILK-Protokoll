using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.util;

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
		[FutureDate(ErrorMessage = "Das Startdatum muss in der Zukunft liegen. ")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime StartDate { get; set; }

		[DisplayName("Bis")]
		[DataType(DataType.Date)]
		[FutureDate(ErrorMessage = "Das Enddatum muss in der Zukunft liegen. ")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime EndDate { get; set; }

		[DisplayName("Uhrzeit")]
		public string Time { get; set; }

		[DisplayName("Ort")]
		public string Place { get; set; }

		[DisplayName("Verant.")]
		public string Organizer { get; set; }

		[DisplayName("Besucher / Thema")]
		public string Description { get; set; }
	}
}