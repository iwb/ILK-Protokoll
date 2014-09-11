using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Administration.Models
{
	[DisplayColumn("Name")]
	public class SessionType
	{
		public SessionType()
		{
			Active = true;
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Attendees = new List<User>();
		}

		public int ID { get; set; }

		[DisplayName("Name")]
		public string Name { get; set; }

		[DisplayName("Auswählbar")]
		public bool Active { get; set; }

		[DisplayName("Letzte Sitzung")]
		public DateTime? LastDate { get; set; }

		[DisplayName("Stammteilnehmer")]
		[InverseProperty("SessionTypes")]
		public virtual ICollection<User> Attendees { get; set; }
	}
}