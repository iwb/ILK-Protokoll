using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Vakente Stellen
	/// </summary>
	[Table("L_Opening")]
	public class Opening : BaseItem
	{
		public string Project { get; set; }

		[DisplayName("Beginn")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime Start { get; set; }

		public string TG { get; set; }

		public Prof Prof { get; set; }

		public string Description { get; set; }
	}
}