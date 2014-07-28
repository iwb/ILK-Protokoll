using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    ILK-Urlaub
	/// </summary>
	[Table("L_Holiday")]
	public class Holiday : BaseItem
	{
		[DisplayName("Person")]
		[Required]
		public User Person { get; set; }

		[Required]
		[DisplayName("Anlass")]
		public string Occasion { get; set; }


		[DisplayName("Beginn")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? Start { get; set; }

		[DisplayName("Ende")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? End { get; set; }
	}
}