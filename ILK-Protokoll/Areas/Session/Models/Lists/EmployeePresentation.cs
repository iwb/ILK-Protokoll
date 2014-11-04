using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Mitarbeiterpräsentation
	/// </summary>
	[Table("L_EmployeePresentation")]
	public class EmployeePresentation : BaseItem
	{
		public EmployeePresentation()
		{
			LastPresentation = DateTime.Today;
			Documents = new List<Document>();
		}

		[Required]
		[DisplayName("Mitarbeiter")]
		public string Employee { get; set; }

		[DisplayName("ILK")]
		public virtual User Ilk { get; set; }

		[ForeignKey("Ilk")]
		public int IlkID { get; set; }

		[DisplayName("Prof.")]
		public Prof Prof { get; set; }

		[DisplayName("Zuletzt")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime LastPresentation { get; set; }

		[DisplayName("Dokumente")]
		public ICollection<Document> Documents { get; set; }

		[DisplayName("Vorgemerkt")]
		public bool Selected { get; set; }

		[NotMapped, SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
		public string FileURL { get; set; }

		[NotMapped]
		public int FileCount { get; set; }

		public override string ToString()
		{
			return string.Format("Mitarbeiter: {0}, ILK: {1}, Prof: {2}", Employee, Ilk, Prof);
		}
	}
}