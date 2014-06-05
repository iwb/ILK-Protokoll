using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Attachment
	{
		public int ID { get; set; }


		[Display(Name = "Diskussion")]
		[InverseProperty("Attachments")]
		public int? TopicID { get; set; }

		[Display(Name = "Dateiname")]
		public string Name { get; set; }

		[Display(Name = "Ersteller")]
		public User Uploader { get; set; }

		[Display(Name = "Uploaddatum")]
		public DateTime Created { get; set; }

		[Display(Name = "Dateigröße")]
		public uint FileSize { get; set; }

		[NotMapped]
		public string FileExtension
		{
			get { return Path.GetExtension(Name); }
		}

		[NotMapped]
		public string URL
		{
			get { return string.Format(@"\\02mucilk\Attachments\{0}_{1}", ID, Name); }
		}
	}
}