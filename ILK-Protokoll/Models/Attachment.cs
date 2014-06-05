using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class Attachment
	{
		public int ID { get; set; }

		[Display(Name = "Diskussion")]
		[InverseProperty("Attachments")]
		public int? TopicID { get; set; }

		/// <summary>
		/// Enthält den Namen, der angezeigt wird. Dieser name kann Zeichen beinhalten, die nicht nicht in Dateinamen zugelassen sind.
		/// </summary>
		[Required]
		[Display(Name = "Name")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Enthält den sicheren Namen der für die Speicherung auf dem Server verwendet wird. Alle unsicheren Zeichen wurden entfernt.
		/// </summary>
		[Required]
		[ScaffoldColumn(false)]
		public string SafeName { get; set; }

		/// <summary>
		/// Enthält die Dateiendung ohne führenden Punkt.
		/// </summary>
		[Required]
		[ScaffoldColumn(false)]
		public string Extension { get; set; }

		[Required]
		[Display(Name = "Ersteller")]
		public User Uploader { get; set; }

		[Required]
		[Display(Name = "Uploaddatum")]
		public DateTime Created { get; set; }

		[Required]
		[Display(Name = "Dateigröße")]
		public int FileSize { get; set; }

		public string FileName
		{
			get { return ID + "_" + SafeName + '.' + Extension; }
		}
	}
}