using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class Revision
	{
		public int ID { get; set; }
		public Guid GUID { get; set; }

		public Document ParentDocument { get; set; }

		[InverseProperty("Revisions")]
		[ForeignKey("ParentDocument")]
		public int ParentDocumentID { get; set; }

		[Required]
		[Display(Name = "Uploaddatum")]
		public DateTime Created { get; set; }

		[Display(Name = "Autor")]
		public virtual User Uploader { get; set; }

		[ForeignKey("Uploader")]
		public int UploaderID { get; set; }


		[Required]
		[Display(Name = "Dateigröße")]
		[UIHint("FileSize")]
		public int FileSize { get; set; }


		/// <summary>
		///    Enthält den sicheren Namen der für die Speicherung auf dem Server verwendet wird. Alle unsicheren Zeichen wurden
		///    entfernt.
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[ScaffoldColumn(false)]
		public string SafeName { get; set; }

		/// <summary>
		///    Enthält die Dateiendung ohne führenden Punkt.
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[ScaffoldColumn(false)]
		public string Extension { get; set; }

		/// <summary>
		/// Enthält den Namen der Datei im Dateisystem.
		/// </summary>
		public string FileName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Extension))
					return ID + "_" + SafeName;
				else
					return ID + "_" + SafeName + '.' + Extension;
			}
		}
	}
}