using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	/// Mitarbeiterpräsentation
	/// </summary>
	[Table("L_EmployeePresentation")]
	public class EmployeePresentation : BaseItem
	{
		public string Employee { get; set; }

		public User Ilk { get; set; }

		public Prof Prof { get; set; }

		public DateTime LastPresentation { get; set; }

		public ICollection<Attachment> Attachments { get; set; }

		public bool Selected { get; set; }
	}
}