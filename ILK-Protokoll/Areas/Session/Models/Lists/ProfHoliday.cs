using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Professorenurlaub
	/// </summary>
	[Table("L_ProfHoliday")]
	public class ProfHoliday : BaseItem
	{
		public Prof Professor { get; set; }

		public string Occasion { get; set; }

		public DateTime? Start { get; set; }

		public DateTime? End { get; set; }

		public override string ToString()
		{
			if (Start == null || End == null)
				return "Kein Urlaub";
			else
				return string.Format("{0:} bis {1}", Start.Value, End.Value);
		}
	}

	public enum Prof
	{
		Zäh, Reinhart
	}
}