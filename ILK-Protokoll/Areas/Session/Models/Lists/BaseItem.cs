using System;
using System.ComponentModel;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	public class BaseItem
	{
		public BaseItem()
		{
			Created = DateTime.Now;
		}

		public int ID { get; set; }

		[DisplayName("Erstellt")]
		public DateTime Created { get; set; }
	}
}