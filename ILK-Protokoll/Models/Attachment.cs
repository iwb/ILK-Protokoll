using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Attachment
	{
		public int ID { get; set; }

		[Display(Name = "Dateiname")]
		public string Name { get; set; }

		[Display(Name = "Ersteller")]
		public User Uploader { get; set; }

		[Display(Name = "Uploaddatum")]
		public DateTime Created { get; set; }

		[NotMapped]
		public string URL
		{
			get { return string.Format(@"\\02mucilk\Attachments\{0}_{1}", ID, Name); }
		}
	}
}