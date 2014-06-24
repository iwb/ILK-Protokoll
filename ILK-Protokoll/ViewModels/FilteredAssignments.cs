using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class FilteredAssignments
	{
		public FilteredAssignments()
		{
			ShowPast = true;
			ShowFuture = true;
			ShowDone = false;
			UserID = 0;
		}

		[DisplayName("Überfällig")]
		public bool ShowPast { get; set; }

		[DisplayName("Zukünftig")]
		public bool ShowFuture { get; set; }

		[DisplayName("Erledigt")]
		public bool ShowDone { get; set; }

		[DisplayName("Benutzer")]
		public int UserID { get; set; }

		public ICollection<Assignment> Assignments { get; set; }
	}
}