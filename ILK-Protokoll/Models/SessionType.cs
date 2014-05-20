using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	[DisplayColumn("Name")]
	public class SessionType
	{
		public int ID { get; set; }
		public string Name { get; set; }
	}
}