using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class FilteredTopics
	{
		public FilteredTopics()
		{
			ShowReadonly = false;
			ShowPriority = -1;
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Topics = new List<Topic>();
		}

		[DisplayName("Sitzungstyp")]
		public int SessionTypeID { get; set; }

		public IEnumerable<SelectListItem> SessionTypeList { get; set; }

		[DisplayName("Schreibgeschützte")]
		public bool ShowReadonly { get; set; }

		public int ShowPriority { get; set; }

		public IEnumerable<SelectListItem> PriorityList { get; set; }

		public int Timespan { get; set; }

		public IEnumerable<SelectListItem> TimespanList { get; set; }

		public virtual ICollection<Topic> Topics { get; set; }
	}
}