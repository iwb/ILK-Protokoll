using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class Attachment
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public string URL
		{
			get { return string.Format(@"\\02mucilk\Attachments\{0}_{1}", ID, Name); }
		}
	}
}