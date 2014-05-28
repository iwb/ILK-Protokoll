using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Decision
	{
		public int ID { get; set; }

		public string Name { get; set; }

		[Required]
		[InverseProperty("Decision")]
		public virtual Topic OriginTopic { get; set; }
	}
}