using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Vakente Stellen
	/// </summary>
	public class Opening : BaseItem
	{
		public string Project { get; set; }

		public DateTime Start { get; set; }

		public string TG { get; set; }

		public Prof Prof { get; set; }

		public string Description { get; set; }

	}
}