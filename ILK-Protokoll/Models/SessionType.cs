using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
		public ICollection<User> Attendees { get; set; }
	}
}