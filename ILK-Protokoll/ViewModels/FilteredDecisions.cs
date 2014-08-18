using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class FilteredDecisions
	{
		public FilteredDecisions()
		{
			ShowClosed = false;
			ShowResolution = true;
			Searchterm = "";
			Timespan = 0;
			WindowStartDate = DateTime.Today.AddDays(-3);
			WindowEndDate = DateTime.Today;

// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Decisions = new List<Decision>();
		}

		[DisplayName("Archiviert")]
		public bool ShowClosed { get; set; }

		[DisplayName("Beschluss")]
		public bool ShowResolution { get; set; }

		[DisplayName("Suchbegriff")]
		public string Searchterm { get; set; }

		/// <summary>
		///    Zeitspanne in Tagen, die durchsucht werden soll. 0, wenn keine Einschränkung getroffen werden soll. -1 wenn das
		///    Zeitfenster über die Properties WindowStartDate und WindowEndDate engegebn wird.
		/// </summary>
		public int Timespan { get; set; }

		public IEnumerable<SelectListItem> TimespanChoices { get; set; }

		[DisplayName("Von")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime WindowStartDate { get; set; }

		[DisplayName("Bis")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime WindowEndDate { get; set; }

		public virtual ICollection<Decision> Decisions { get; set; }
	}
}