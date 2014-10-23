using System.Collections;
using System.Collections.Generic;

namespace ILK_Protokoll.ViewModels
{
	public class SearchResultList : IEnumerable<SearchResult>
	{
		private readonly List<SearchResult> _results = new List<SearchResult>();
		private readonly Dictionary<int, SearchResult> _topics = new Dictionary<int, SearchResult>();

		public int Count
		{
			get { return _results.Count; }
		}

		public SearchResult this[int topicID]
		{
			get { return _topics[topicID]; }
		}

		public void Add(SearchResult item)
		{
			_results.Add(item);
		}

		public void Add(int topicID, SearchResult item)
		{
			_topics.Add(topicID, item);
			_results.Add(item);
		}

		public void AddHit(int topicID, Hit item)
		{
			_topics[topicID].Hits.Add(item);
		}
		public void AddHits(int topicID, IEnumerable<Hit> items)
		{
			foreach (var item in items)
				_topics[topicID].Hits.Add(item);
		}

		public void Clear()
		{
			_topics.Clear();
			_results.Clear();
		}

		public bool Contains(SearchResult item)
		{
			return _results.Contains(item);
		}

		public bool Contains(int topicID)
		{
			return _topics.ContainsKey(topicID);
		}

		public void Sort()
		{
			_results.Sort((a, b) => b.Score.CompareTo(a.Score));
		}

		public IEnumerator<SearchResult> GetEnumerator()
		{
			return _results.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _results.GetEnumerator();
		}
	}
}