using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Vertragsverlängerungen
	/// </summary>
	public class Extension : BaseItem
	{
		public string Employee { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		[Range(1, 3)]
		public int ExtensionNumber { get; set; }

		public string Comment { get; set; }

		public bool Approved { get; set; }
	}
}