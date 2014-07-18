using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models
{
	public class ActiveSession
	{
		public ActiveSession()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			PresentUsers = new List<User>();
			LockedTopics = new List<TopicLock>();
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
			Start = DateTime.Now;
		}

		public ActiveSession(SessionType type)
			: this()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			SessionType = type;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public int ID { get; set; }

		[DisplayName("Sitzungsleiter")]
		[Required]
		public User Manager { get; set; }

		[DisplayName("Sitzungstyp")]
		[Required]
		public virtual SessionType SessionType { get; set; }

		[DisplayName("Anwesenheit")]
		[Required]
		public virtual ICollection<User> PresentUsers { get; set; }

		[DisplayName("Weitere Personen")]
		public string AdditionalAttendees { get; set; }

		[DisplayName("Notizen")]
		[DataType(DataType.MultilineText)]
		public string Notes { get; set; }

		[DisplayName("Beginn")]
		[Required]
		public DateTime Start { get; set; }

		public virtual ICollection<TopicLock> LockedTopics { get; set; }
	}
}