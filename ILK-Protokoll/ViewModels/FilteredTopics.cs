using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	/// <summary>
	///    Das ViewModel für die Themenliste.
	/// </summary>
	public class FilteredTopics
	{
		/// <summary>
		///    Konstruiert den Filter so, dass alle aktuellen Diskussionen angezeigt werden. Archivierte Diskussionen werden
		///    ausgeblendet.
		/// </summary>
		public FilteredTopics()
		{
			ShowReadonly = false;
			ShowPriority = -1;
			ShowTagsID = new List<int>();
			Topics = new List<Topic>();
		}

		/// <summary>
		///    Der Sitzungstyp, auf den die Liste beschränkt werden soll. 0, wenn keine Beschränkugn stattfindet.
		/// </summary>
		[DisplayName("Sitzungstyp")]
		public int SessionTypeID { get; set; }

		/// <summary>
		///    Alle möglichen Sitzungstypen für das Dropdown-Menü.
		/// </summary>
		public IEnumerable<SelectListItem> SessionTypeList { get; set; }

		/// <summary>
		///    Gibt an, ob schreibgeschützte Themen angezeigt werden sollen.
		/// </summary>
		[DisplayName("Schreibgeschützte")]
		public bool ShowReadonly { get; set; }

		/// <summary>
		///    Die Priorität, auf die die Liste beschränkt werden soll. -1, falls keine Beschränkung.
		/// </summary>
		public int ShowPriority { get; set; }

		/// <summary>
		///    Alle Prioritäten für das Dropdown-Menü.
		/// </summary>
		public IEnumerable<SelectListItem> PriorityList { get; set; }

		/// <summary>
		///    Gibt den Index der Zeitspanne an, auf die die Liste beschränkt werden soll.
		/// </summary>
		public int Timespan { get; set; }

		/// <summary>
		///    Alle Zeitspannen für das Dropdown-Menü.
		/// </summary>
		public IEnumerable<SelectListItem> TimespanList { get; set; }

		/// <summary>
		///    Der Besitzer, auf den die Anzeige eingeschränkt werden soll.
		/// </summary>
		public int OwnerID { get; set; }

		/// <summary>
		///    Alle Benutzer für für das Dropdown-Menü.
		/// </summary>
		public SelectList UserList { get; set; }

		/// <summary>
		///    Die Tags, auf den die Anzeige eingeschränkt werden soll.
		/// </summary>
		public ICollection<int> ShowTagsID { get; set; }

		/// <summary>
		///    Liste aller Tags für das Dropdown-Menü.
		/// </summary>
		public IEnumerable<SelectListItem> TagList { get; set; }

		/// <summary>
		///    Themenliste, die angezeigt wird. In der Regel das Ergebnis der Filterung.
		/// </summary>
		public ICollection<Topic> Topics { get; set; }
	}
}