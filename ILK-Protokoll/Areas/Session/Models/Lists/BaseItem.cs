using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	public class BaseItem
	{
		protected BaseItem()
		{
			Created = DateTime.Now;
		}

		public int ID { get; set; }

		[DisplayName("Erstellt")]
		public DateTime Created { get; set; }



	}

}