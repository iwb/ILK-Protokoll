using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Klausur-Tage
	/// </summary>
	[Table("L_IlkDay")]
	public class IlkDay : BaseItem
	{

		public DateTime Start { get; set; }

		public string Place { get; set; }

		public SessionType SessionType { get; set; }

		public User Organizer { get; set; }

		public string Topics { get; set; }

		public string Participants { get; set; }
	}
}