using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	[DisplayColumn("Name")]
	public class SessionType
	{
		public SessionType()
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Attendees = new List<User>();
		}

		public int ID { get; set; }

		public string Name { get; set; }

		[DisplayName("Stammteilnehmer")]
		[InverseProperty("SessionTypes")]
		public virtual ICollection<User> Attendees { get; set; }
	}
}