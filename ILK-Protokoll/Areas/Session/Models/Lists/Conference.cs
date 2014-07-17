using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Auslandskonferenzen
	/// </summary>
	[Table("L_Conference")]
	public class Conference : BaseItem
	{
		public Conference()
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

		[DisplayName("Name / Ort")]
		public string Description { get; set; }

		[DisplayName("ILK")]
		public virtual User Ilk { get; set; }

		[ForeignKey("Ilk")]
		public int IlkID { get; set; }

		[DisplayName("Mitarbeiter")]
		public string Employee { get; set; }

		[DisplayName("Budget")]
		public string Funding { get; set; }

		[DisplayName("Genehmigt")]
		public bool Approved { get; set; }
	}
}