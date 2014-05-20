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
				Owner = users[4],
				Title = "Mehr Kekse",
				Proposal = " Zukünftige Dissertationsthemen werden nur zugelassen, falls ein Zusammenhang mit Keksen deutlich erkennbar ist.",
				Description = "Das iwb sollte mehr Kekse backen.",
				Priority = Priority.High
			};


			topic.Votes.Add(new Vote(users[0], VoteKind.Approved));
			topic.Votes.Add(new Vote(users[1], VoteKind.Approved));
			topic.Votes.Add(new Vote(users[2], VoteKind.None));
			topic.Votes.Add(new Vote(users[3], VoteKind.Rejected));
			topic.Votes.Add(new Vote(users[4], VoteKind.Reservation));
			topic.Votes.Add(new Vote(users[5], VoteKind.None));
			topic.Votes.Add(new Vote(users[6], VoteKind.Rejected));
			topic.Votes.Add(new Vote(users[7], VoteKind.Reservation));
			topic.Votes.Add(new Vote(users[8], VoteKind.None));
			topic.Votes.Add(new Vote(users[9], VoteKind.Approved));
			topic.Votes.Add(new Vote(users[10], VoteKind.None));

			topic.Comments.Add(new Comment() { Author = users[0], Content = "Klingt lecker! und ich finde den Beschlussvorschlag auch total sinnvoll und angemessen. Eine Dissertation, deren Ergebnisse man nicht essen oder trinken kann, ist im Grunde wertlos. Insbesondere am Institut für Weißwurscht und Brezenwissenschaften." });

			topic.Comments.Add(new Comment() { Author = users[4], Content = "Brauchen wir nicht drüber reden. Ist gegessen." });
			topic.Comments.Add(new Comment() { Author = users[8], Content = "Möglicherweise sollten auch Themen im Komplex \"Backplanung und -steuerung\" erlaubt werden." });

			var sessiont = new SessionType() {ID = 1, Name = "ILK-AK Garching"};
			topic.SessionType = sessiont;



			context.Users.AddRange(users);
			context.SaveChanges();

			context.Topics.Add(topic);
			context.SessionTypes.Add(sessiont);
			context.SaveChanges();
		}
	}
}