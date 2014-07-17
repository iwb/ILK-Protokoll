using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.util;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Vakente Stellen
	/// </summary>
	[Table("L_Opening")]
	public class Opening : BaseItem
	{
		public Opening()
		{
			Start = DateTime.Today;
		}

		[DisplayName("Projekt")]
		public string Project { get; set; }

		[DisplayName("Beginn")]
		[DataType(DataType.Date)]
		[FutureDate]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Start { get; set; }

		public string TG { get; set; }

		[DisplayName("Prof.")]
		public Prof Prof { get; set; }

		[DisplayName("Beschreibung / Profil")]
		public string Description { get; set; }
	}
}