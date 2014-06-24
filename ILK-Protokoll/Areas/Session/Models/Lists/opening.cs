using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Vakente Stellen
	/// </summary>
	[Table("L_Opening")]
	public class Opening : BaseItem
	{
		public string Project { get; set; }

		public DateTime Start { get; set; }

		public string TG { get; set; }

		public Prof Prof { get; set; }

		public string Description { get; set; }

	}
}