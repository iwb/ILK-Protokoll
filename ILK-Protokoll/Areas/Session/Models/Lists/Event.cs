using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Termine und Veranstaltungen, die das ganze Institut betreffen
	/// </summary>
	[Table("L_Event")]
	public class Event : BaseItem
	{

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime? EndDate { get; set; }

		[DisplayName("Uhrzeit")]
		public string Time { get; set; }

		[DisplayName("Ort")]
		public string Place { get; set; }

		[DisplayName("Verantwortlich")]
		public string Organizer { get; set; }

		[DisplayName("Besucher / Thema")]
		public string Description { get; set; }
	}
}