using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	[DisplayColumn("DisplayName")]
	public class Attachment
	{
		public int ID { get; set; }

		[Display(Name = "Diskussion")]
		[InverseProperty("Attachments")]
		public int? TopicID { get; set; }

		[ForeignKey("TopicID")]
		public virtual Topic Topic { get; set; }

		/// <summary>
		/// Enthält das löschdatum, falls der Anhang gelöscht wurde, sonst null.
		/// </summary>
		[Display(Name = "Gelöscht")]
		public DateTime? Deleted { get; set; }

		/// <summary>
		/// Enthält den Namen, der angezeigt wird. Dieser name kann Zeichen beinhalten, die nicht nicht in Dateinamen zugelassen sind.
		/// </summary>
		[Required]
		[Display(Name = "Name")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Enthält den sicheren Namen der für die Speicherung auf dem Server verwendet wird. Alle unsicheren Zeichen wurden entfernt.
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[ScaffoldColumn(false)]
		public string SafeName { get; set; }

		/// <summary>
		/// Enthält die Dateiendung ohne führenden Punkt.
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		[ScaffoldColumn(false)]
		public string Extension { get; set; }

		[Required]
		[Display(Name = "Ersteller")]
		public virtual User Uploader { get; set; }

		[Required]
		[Display(Name = "Uploaddatum")]
		public DateTime Created { get; set; }

		[Required]
		[Display(Name = "Dateigröße")]
		[UIHint("FileSize")]
		public int FileSize { get; set; }

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