using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Session.Models.Lists;

namespace ILK_Protokoll.Models
{
	[DisplayColumn("DisplayName")]
	public class Document
	{
		public Document()
		{
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Revisions = new List<Revision>();
		}
		public int ID { get; set; }
		
		public Guid Guid { get; set; }

		/// <summary>
		/// Enthält das Lockdatum, falls das Dokument gesperrt ist, sonst null.
		/// </summary>
		public DateTime? LockTime { get; set; }

		[Required]
		[Display(Name = "Erstelldatum")]
		public DateTime Created { get; set; }

		/// <summary>
		///    Enthält das Löschdatum, falls der Anhang gelöscht wurde, sonst null.
		/// </summary>
		[Display(Name = "Gelöscht")]
		public DateTime? Deleted { get; set; }

		/// <summary>
		///    Enthält die Dateiendung ohne führenden Punkt.
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[ScaffoldColumn(false)]
		public string Extension { get; set; }

		/// <summary>
		///    Enthält den Namen, der angezeigt wird. Dieser Name kann Zeichen beinhalten,
		///    die nicht nicht in Dateinamen zugelassen sind.
		/// </summary>
		[Required]
		[Display(Name = "Name")]
		public string DisplayName { get; set; }

		public virtual ICollection<Revision> Revisions { get; set; }
	}

	public class TopicAttachment : Document
	{
		[Display(Name = "Diskussion")]
		[InverseProperty("Attachments")]
		public int? TopicID { get; set; }

		[ForeignKey("TopicID")]
		public virtual Topic Topic { get; set; }
	}

	public class EmployeePresentationAttachment : Document
	{
		[Display(Name = "Präsentation")]
		[InverseProperty("Attachments")]
		public int? EmployeePresentationID { get; set; }

		[ForeignKey("EmployeePresentationID")]
		public virtual EmployeePresentation EmployeePresentation { get; set; }
	}
}