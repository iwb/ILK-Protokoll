using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	[DisplayColumn("Name")]
	public class SessionType
	{
		public SessionType()
		{
			Attendees = new List<User>();
		}

		public int ID { get; set; }

		public string Name { get; set; }

		[InverseProperty("SessionTypes")]
		public virtual ICollection<User> Attendees { get; set; }
	}
}