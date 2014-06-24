using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// ILK-Regeltermine
	/// </summary>
	public class IlkMeeting : BaseItem
	{
		public DateTime Start { get; set; }

		public string Place { get; set; }

		public SessionType SessionType { get; set; }

		public User Organizer { get; set; }

		public string Comments { get; set; }
	}
}