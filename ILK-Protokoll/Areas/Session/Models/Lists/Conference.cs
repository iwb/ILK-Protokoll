using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Auslandskonferenzen
	/// </summary>
	[Table("L_Conference")]
	public class Conference : BaseItem
	{
		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime EndDate { get; set; }

		public string Description { get; set; }

		public User Ilk { get; set; }

		public string Employee { get; set; }

		public string Funding { get; set; }

		public bool Approved { get; set; }
	}
}