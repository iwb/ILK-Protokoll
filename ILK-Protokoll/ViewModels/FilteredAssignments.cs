using System.Collections.Generic;
using System.ComponentModel;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class FilteredAssignments
	{
		public FilteredAssignments()
		{
			ShowToDos = true;
			ShowDuties = true;
			ShowPast = true;
			ShowFuture = true;
			ShowDone = false;
			UserID = 0;
			Assignments = new List<Assignment>();
		}

		[DisplayName("ToDo's")]
		public bool ShowToDos { get; set; }

		[DisplayName("Umsetzungsaufgaben")]
		public bool ShowDuties { get; set; }

		[DisplayName("Überfällig")]
		public bool ShowPast { get; set; }

		[DisplayName("Zukünftig")]
		public bool ShowFuture { get; set; }

		[DisplayName("Erledigt")]
		public bool ShowDone { get; set; }

		[DisplayName("Benutzer")]
		public int UserID { get; set; }

		public virtual ICollection<Assignment> Assignments { get; set; }
	}
}