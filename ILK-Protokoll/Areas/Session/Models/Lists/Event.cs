using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Termine und Veranstaltungen, die das ganze Institut betreffen
	/// </summary>
	public class Event : BaseItem
	{

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }

		[DataType(DataType.Date)]
		public DateTime? EndDate { get; set; }

		public string Time { get; set; }

		public string Place { get; set; }

		public User Organizer { get; set; }

		public string Description { get; set; }
	}
}