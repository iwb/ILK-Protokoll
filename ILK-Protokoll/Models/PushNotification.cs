using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class PushNotification
	{
		public int ID { get; set; }

		[ForeignKey("User")]
		public int UserID { get; set; }

		[Display(Name = "Zur Kenntnisnahme pushen an")]
		public virtual User User { get; set; }

		[ForeignKey("Topic")]
		public int TopicID { get; set; }

		[Display(Name = "Thema")]
		public virtual Topic Topic { get; set; }

		public bool Confirmed { get; set; }
	}
}