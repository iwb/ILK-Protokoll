using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Models;

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
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime StartDate { get; set; }

		[DisplayName("Bis")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime EndDate { get; set; }

		[Required]
		[DisplayName("Name / Ort")]
		public string Description { get; set; }

		[DisplayName("ILK")]
		public virtual User Ilk { get; set; }

		[ForeignKey("Ilk")]
		public int IlkID { get; set; }

		[Required]
		[DisplayName("Mitarbeiter")]
		public string Employee { get; set; }

		[Required(AllowEmptyStrings = true)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		[DisplayName("Budget")]
		public string Funding { get; set; }

		[DisplayName("Genehmigt")]
		public Approval Approval { get; set; }
	}
}