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
		public ActiveSession()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			PresentUsers = new Dictionary<User, bool>();
			DecidedTopics = new List<Topic>();
			ChangedItems = new HashSet<object>();
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
			Start = DateTime.Now;
		}
		public ActiveSession(SessionType type)
			: this()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			SessionType = type;
			PresentUsers = type.Attendees.ToDictionary(user => user, u => false);
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public int ID { get; set; }

		[DisplayName("Sitzungsleiter")]
		public User Manager { get; set; }

		[DisplayName("Sitzungstyp")]
		public virtual SessionType SessionType { get; set; }

		[DisplayName("Anwesenheit")]
		[UIHint("Dictionary_User_bool")]
		public virtual Dictionary<User, bool> PresentUsers { get; set; }

		[DisplayName("Weitere Personen")]
		public string AdditionalAttendees { get; set; }

		[DisplayName("Notizen")]
		[DataType(DataType.MultilineText)]
		public string Notes { get; set; }

		[DisplayName("Beginn")]
		public DateTime Start { get; set; }

		public virtual ICollection<Topic> DecidedTopics { get; set; }

		public virtual HashSet<object> ChangedItems { get; set; }
	}
}