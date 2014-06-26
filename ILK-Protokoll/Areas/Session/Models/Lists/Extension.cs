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
		[DisplayName("Mitarbeiter")]
		public string Employee { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime EndDate { get; set; }

		[Range(1, 3)]
		public int ExtensionNumber { get; set; }

		[DisplayName("Kommentar")]
		public string Comment { get; set; }

		[DisplayName("Genehmigt")]
		public bool Approved { get; set; }
	}
}