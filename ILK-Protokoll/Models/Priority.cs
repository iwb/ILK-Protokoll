using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public enum Priority
	{
		[Display(Name= "Niedrig")]
		Low,
		[Display(Name = "Mittel")]
		Medium,
		[Display(Name = "Hoch")]
		High
	}
}