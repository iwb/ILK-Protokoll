using System;
using System.Collections.Generic;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.ViewModels
{
	public class TopicHistoryViewModel
	{
		public TopicHistoryViewModel()
		{
			Usernames = new Dictionary<int, string>();
			SessionTypes = new Dictionary<int, string>();
			Differences = new List<TopicHistoryDiff>();
		}

		public Topic Current { get; set; }

		public TopicHistory Initial { get; set; }

		public Dictionary<int, string> Usernames { get; set; }

		public Dictionary<int, string> SessionTypes { get; set; }

		public IList<TopicHistoryDiff> Differences { get; set; }
	}

	public class TopicHistoryDiff
	{
		public DateTime Modified { get; set; }

		public string Editor { get; set; }

		public string SessionType { get; set; }

		public string TargetSessionType { get; set; }

		public string Time { get; set; }

		public string Owner { get; set; }

		public string Priority { get; set; }

		public ICollection<Diff> Title { get; set; }
		public ICollection<Diff> Description { get; set; }
		public ICollection<Diff> Proposal { get; set; }
	}
}