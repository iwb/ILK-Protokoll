﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	/// <summary>
	///    Mitarbeiterpräsentation
	/// </summary>
	[Table("L_EmployeePresentation")]
	public class EmployeePresentation : BaseItem
	{
		[DisplayName("Mitarbeiter")]
		public string Employee { get; set; }

		[DisplayName("ILK")]
		public User Ilk { get; set; }

		[DisplayName("Prof.")]
		public Prof Prof { get; set; }

		[DisplayName("Zuletzt")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime LastPresentation { get; set; }

		public ICollection<Attachment> Attachments { get; set; }

		public bool Selected { get; set; }
	}
}