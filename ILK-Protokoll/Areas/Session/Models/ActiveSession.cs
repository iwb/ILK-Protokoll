using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models
{
	public class ActiveSession
	{
		public ActiveSession(SessionType type)
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			SessionType = type;
			PresentUsers = type.Attendees.ToDictionary(user => user, u => false);
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
			Start = DateTime.Now;
		}

		public int ID { get; set; }

		[DisplayName("Sitzungstyp")]
		public virtual SessionType SessionType { get; set; }

		[DisplayName("Anwesenheit")]
		[UIHint("Dictionary_User_bool")]
		public virtual Dictionary<User, bool> PresentUsers { get; set; }

		[DisplayName("Weitere Personen")]
		public string AdditionalAttendees { get; set; }

		[DisplayName("Notizen")]
		public string Notes { get; set; }

		[DisplayName("Beginn")]
		public DateTime Start { get; set; }
	}
}