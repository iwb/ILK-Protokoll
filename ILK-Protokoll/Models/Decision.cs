using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class Decision
	{
		public int ID { get; set; }

		[Required]
		public virtual SessionReport Report { get; set; }

		[DisplayName("Art")]
		public DecisionType Type { get; set; }

		[DataType(DataType.MultilineText)]
		[DisplayName("Beschlusstext")]
		public string Text { get; set; }

		[Required]
		[InverseProperty("Decision")]
		[DisplayName("Diskussionstitel")]
		public virtual Topic OriginTopic { get; set; }
	}

	public enum DecisionType
	{
		[Display(Name = "Archiviert")] Closed,
		[Display(Name = "Beschlossen")] Resolution
	}
}