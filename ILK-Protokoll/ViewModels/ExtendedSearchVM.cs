using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.ViewModels
{
	public class ExtendedSearchVM
	{
		public ExtendedSearchVM()
		{
			SearchTopics = true;
			SearchComments = true;
			SearchAssignments = true;
			SearchAttachments = true;
			SearchDecisions = true;
			SearchLists = true;
		}

		[DisplayName("Suchbegriff")]
		public string Searchterm { get; set; }

		[DisplayName("Diskussionen")]
		public bool SearchTopics { get; set; }

		[DisplayName("Kommentare")]
		public bool SearchComments { get; set; }

		[DisplayName("Aufgaben")]
		public bool SearchAssignments { get; set; }

		[DisplayName("Anhänge")]
		public bool SearchAttachments { get; set; }

		[DisplayName("Entscheidungen")]
		public bool SearchDecisions { get; set; }

		[DisplayName("Listeneinträge")]
		public bool SearchLists { get; set; }
	}
}