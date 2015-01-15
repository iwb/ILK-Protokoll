using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.ViewModels
{
	public class AssignmentEdit
	{
		public AssignmentEdit()
		{
			DueDate = DateTime.Today;
			IsActive = true;
		}

		public int ID { get; set; }

		[Display(Name = "Typ")]
		public AssignmentType Type { get; set; }

		[Display(Name = "Titel")]
		[Required]
		public string Title { get; set; }

		[Display(Name = "Beschreibung")]
		[DataType(DataType.MultilineText)]
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public string Description { get; set; }

		[Display(Name = "Diskussion")]
		public int TopicID { get; set; }

		[Display(Name = "Besitzer")]
		public int OwnerID { get; set; }

		[Display(Name = "Deadline")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[FutureDate(ErrorMessage = "Die Deadline muss in der Zukunft liegen.")]
		[DataType(DataType.Date)]
		[Required]
		public DateTime DueDate { get; set; }

		[Display(Name = "Aktiv")]
		public bool IsActive { get; set; }

		public IEnumerable<SelectListItem> OwnerSelectList { get; set; }
	}
}