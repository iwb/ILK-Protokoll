using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ILK_Protokoll.ViewModels
{
	public class SearchResult : IEquatable<SearchResult>
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

		public int ID { get; set; }

		public float Score { get; set; }

		public string EntityType { get; set; }

		public string Title { get; set; }

		public ICollection<Hit> Hits { get; set; }
		
		public string ActionURL { get; set; }

		public DateTime? Timestamp { get; set; }

		public bool Equals(SearchResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ID == other.ID && string.Equals(Title, other.Title) && string.Equals(EntityType, other.EntityType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			var other = obj as SearchResult;
			return other != null && Equals(other);
		}

		public override int GetHashCode()
		{
			return ID;
		}

		public static bool operator ==(SearchResult left, SearchResult right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SearchResult left, SearchResult right)
		{
			return !Equals(left, right);
		}
	}

	public struct Hit
	{
		public string Property;
		public string Text;
	}
}