using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public enum Priority
	{
		[Display(Name = "(Keine)")]
		None = 0,
		[Display(Name= "Niedrig")]
		Low = -1,
		[Display(Name = "Mittel")]
		Medium = 1,
		[Display(Name = "Hoch")]
		High = 2
	}
}