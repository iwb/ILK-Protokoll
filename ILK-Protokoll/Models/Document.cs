using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Session.Models.Lists;
using Microsoft.Ajax.Utilities;

namespace ILK_Protokoll.Models
{
	[DisplayColumn("DisplayName")]
	public class Document
	{
		public Document()
		{
			LockTime = null;
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Revisions = new List<Revision>();
		}

		public Document(Revision file) : this()
		{
// ReSharper disable DoNotCallOverridableMethodsInConstructor
			GUID = Guid.NewGuid();
			Revisions.Add(file);
			Created = DateTime.Now;
			file.ParentDocument = this;
// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public int ID { get; set; }
		
		public Guid GUID { get; set; }

		//----------------------------------------------------------------------------------------------------
		[Display(Name = "Diskussion")]
		[InverseProperty("Attachments")]
		public int? TopicID { get; set; }

		[ForeignKey("TopicID")]
		public virtual Topic Topic { get; set; }

		//----------------------------------------------------------------------------------------------------

		[Display(Name = "Präsentation")]
		[InverseProperty("Attachments")]
		public int? EmployeePresentationID { get; set; }

		[ForeignKey("EmployeePresentationID")]
		public virtual EmployeePresentation EmployeePresentation { get; set; }

		//----------------------------------------------------------------------------------------------------

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
		///    Enthält den Namen, der angezeigt wird. Dieser Name kann Zeichen beinhalten,
		///    die nicht nicht in Dateinamen zugelassen sind.
		/// </summary>
		[Required]
		[Display(Name = "Name")]
		public string DisplayName { get; set; }

		public virtual ICollection<Revision> Revisions { get; set; }

		public virtual Revision LatestRevision { get; set; }
		[ForeignKey("LatestRevision")]
		public int? LatestRevisionID { get; set; }
	}

	public interface IDocumentContainer
	{
		int ID { get; }
		ICollection<Document> Documents { get; }
	}

	public enum DocumentContainer
	{
		Topic,
		EmployeePresentation
	}
}