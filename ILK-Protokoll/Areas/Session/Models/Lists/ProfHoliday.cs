using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Professorenurlaub
	/// </summary>
	[Table("L_ProfHoliday")]
	public class ProfHoliday : BaseItem
	{
		[DisplayName("Professor")]
		public Prof Professor { get; set; }

		[DisplayName("Anlass")]
		public string Occasion { get; set; }


		[DisplayName("Beginn")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime? Start { get; set; }

		[DisplayName("Ende")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
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
		Zäh,
		Reinhart
	}
}