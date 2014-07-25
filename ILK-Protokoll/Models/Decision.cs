using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class Decision
	{
		public int ID { get; set; }

		[Required]
		public SessionReport Report { get; set; }

		public DecisionType Type { get; set; }

		[DataType(DataType.MultilineText)]
		public string Text { get; set; }

		[Required]
		[InverseProperty("Decision")]
		public virtual Topic OriginTopic { get; set; }
	}

	public enum DecisionType
	{
		Closed,
		Resolution
	}
}