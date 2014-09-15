using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Vertragsverlängerungen
	/// </summary>
	[Table("L_Extension")]
	public class Extension : BaseItem
	{
		public Extension()
		{
			EndDate = DateTime.Today.AddYears(2);
		}

		[Required]
		[DisplayName("Mitarbeiter")]
		public string Employee { get; set; }

		[DisplayName("Vertragsende")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime EndDate { get; set; }

		[DisplayName("1/2/3")]
		[Range(1, 3)]
		public int ExtensionNumber { get; set; }

		[Required]
		[DisplayName("Kommentar")]
		public string Comment { get; set; }

		[DisplayName("Genehmigt")]
		public Approval Approval { get; set; }
	}
}