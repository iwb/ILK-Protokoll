using System.Collections.Generic;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.ViewModels
{
	public class VoteListViewModel
	{
		public VoteListViewModel()
		{
			OtherVotes = new List<Vote>();
		}

		public int TopicID { get; set; }
		public Vote OwnVote { get; set; }
		public VoteLinkLevel LinkLevel { get; set; }
		public IEnumerable<Vote> OtherVotes { get; set; }
	}

	/// <summary>
	///    Bestimmt, welche Icons verlinkt werden, um eine Stimmabgabe zu ermöglichen
	/// </summary>
	public enum VoteLinkLevel
	{
		/// <summary>
		///    Es werden keine Links generiert
		/// </summary>
		None,

		/// <summary>
		///    Es wird nur ei Link zur Abgabe der eigenen Stimme generiert
		/// </summary>
		OnlyMine,

		/// <summary>
		///    Es wird für jeden Stimmberechtigten ein Link generiert
		/// </summary>
		Everyone
	}
}