using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.DataLayer
{
	public class MockupInitializer : DropCreateDatabaseAlways<DataContext>
	{
		protected override void Seed(DataContext context)
		{
			var users = new List<User>()
			{
				new User("hz"),
				new User("sw"),
				new User("gf"),
				new User("ga"),
				new User("mf"),
				new User("kj"),
				new User("kf"),
				new User("ls"),
				new User("re"),
				new User("ro"),
				new User("zh"),
			};

			var topic = new Topic() {
				Owner = users[3],
				Title = "Mehr Kekse",
				Proposal = " Zukünftige Dissertationsthemen werden nur zugelassen, falls ein Zusammenhang mit Keksen deutlich erkennbar ist.",
				Description = "Das iwb sollte mehr Kekse backen.",
				Priority = Priority.High
			};

			foreach (var user in users)
				topic.AuditorList.Add(user);

			topic.Votes.Add(new Vote(users[0], VoteKind.Approved));
			topic.Votes.Add(new Vote(users[1], VoteKind.Approved));
			topic.Votes.Add(new Vote(users[3], VoteKind.Rejected));
			topic.Votes.Add(new Vote(users[4], VoteKind.Reservation));
			topic.Votes.Add(new Vote(users[6], VoteKind.Rejected));
			topic.Votes.Add(new Vote(users[7], VoteKind.Reservation));
			topic.Votes.Add(new Vote(users[9], VoteKind.Approved));

			topic.Comments.Add(new Comment() { Author = users[0], Content = "Klingt lecker! und gut mit dem ''Zukünftige Dissertationsthemen werden nur zugelassen, falls ein Zusammenhang mit Keksen deutlich erkennbar ist''" });

			var sessiont = new SessionType() {ID = 1, Name = "Brotzeit"};
			topic.SessionType = sessiont;

			context.Users.AddRange(users);
			context.SaveChanges();

			context.Topics.Add(topic);
			context.SessionTypes.Add(sessiont);
			context.SaveChanges();
		}
	}
}