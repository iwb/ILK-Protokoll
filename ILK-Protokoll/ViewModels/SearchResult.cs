using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ILK_Protokoll.ViewModels
{
	public class SearchResult
	{
		public SearchResult()
		{
			Hits = new List<Hit>();
		}

		public SearchResult(string hittext) : this()
		{
			Hits.Add(new Hit {Property = "", Text = hittext});
		}

		public SearchResult(string hitname, string hittext)
			: this()
		{
			Hits.Add(new Hit { Property = hitname, Text = hittext });
		}

		public float Score { get; set; }

		public string EntityType { get; set; }

		public string Title { get; set; }

		public ICollection<Hit> Hits { get; set; }
		
		public string ActionURL { get; set; }

		public DateTime? Timestamp { get; set; }
	}

	public struct Hit
	{
		public string Property;
		public string Text;
	}
}