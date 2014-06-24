using System;
using System.Collections.Generic;
using System.Data.Entity;
using ILK_Protokoll.Areas.Administration.Controllers;
using ILK_Protokoll.Areas.Administration.Models;
using ILK_Protokoll.Areas.Session.Models.Lists;
using ILK_Protokoll.Models;

namespace ILK_Protokoll.DataLayer
{
	public class MockupInitializer : DropCreateDatabaseIfModelChanges<DataContext>
	{
		protected override void Seed(DataContext context)
		{
			var users = new List<User>
			{
				UserController.CreateUserFromShortName("hz"),
				UserController.CreateUserFromShortName("sw"),
				UserController.CreateUserFromShortName("gf"),
				UserController.CreateUserFromShortName("ga"),
				UserController.CreateUserFromShortName("mf"),
				UserController.CreateUserFromShortName("kj"),
				UserController.CreateUserFromShortName("kf"),
				UserController.CreateUserFromShortName("ls"),
				UserController.CreateUserFromShortName("re"),
				UserController.CreateUserFromShortName("ro"),
				UserController.CreateUserFromShortName("zh"),
				UserController.CreateUserFromShortName("hm")
			};

			var topic = new Topic
			{
				Owner = users[4],
				Title = "Mehr Kekse",
				Proposal =
					"Zukünftige Dissertationsthemen werden nur zugelassen, falls ein Zusammenhang mit Keksen deutlich erkennbar ist.",
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
			topic.Votes.Add(new Vote(users[11], VoteKind.Reservation));

			topic.Comments.Add(new Comment
			{
				Author = users[0],
				Content =
					"Klingt lecker! und ich finde den Beschlussvorschlag auch total sinnvoll und angemessen. Eine Dissertation, deren Ergebnisse man nicht essen oder trinken kann, ist im Grunde wertlos. Insbesondere am Institut für Weißwurscht und Brezenwissenschaften."
			});

			topic.Comments.Add(new Comment { Author = users[4], Content = "Brauchen wir nicht drüber reden. Ist gegessen." });
			topic.Comments.Add(new Comment
			{
				Author = users[8],
				Content = "Möglicherweise sollten auch Themen im Komplex \"Backplanung und -steuerung\" erlaubt werden."
			});

			var sessiont = new SessionType { ID = 1, Name = "ILK-AK Garching" };
			sessiont.Attendees.Add(users[9]);
			sessiont.Attendees.Add(users[1]);
			sessiont.Attendees.Add(users[4]);
			sessiont.Attendees.Add(users[5]);
			topic.SessionType = sessiont;

			context.Users.AddRange(users);
			context.SaveChanges();

			context.Topics.Add(topic);
			context.SessionTypes.Add(sessiont);

			var st2 = new SessionType() { Name = "ILK-AK Augsburg" };
			st2.Attendees.Add(users[0]);
			st2.Attendees.Add(users[1]);
			st2.Attendees.Add(users[3]);
			st2.Attendees.Add(users[6]);
			st2.Attendees.Add(users[7]);
			st2.Attendees.Add(users[8]);
			context.SessionTypes.Add(st2);

			context.L_Events.Add(new Event()
			{
				Created = DateTime.Now.AddHours(-1),
				Description = "TG-Tag",
				StartDate = DateTime.Parse("2014-07-11"),
				EndDate = DateTime.Parse("2014-07-11"),
				Place = "Legoland",
				Organizer = "hm, hz",
				Time = "ganztags"
			});

			topic = new Topic
			{
				Owner = users[2],
				Title = "Gleichberechtigung Kuchen <=> Kekse",
				Proposal =
					"Kuchen und Kekse sind in allen belangen gleichberechtigt. Eine Gewährung von Vorteilen oder Benachteiligung eines Gebäcks aufgrund seiner Beschaffenheit ist in jedem Fall zu unterlassen.",
				Description =
					"Eine Rücksprache mit der Gleichstellungsbeauftragten ergab, dass eine Förderung von Keksen gegen den Gleichstellungsgrundsatz verstößt. Daher sollte explizit klargestellt werden, dass beide Gebäckarten gleichberechtigt sind.",
				Priority = Priority.High,
				SessionType = sessiont
			};

			foreach (User u in users)
				topic.Votes.Add(new Vote(u, VoteKind.None));

			context.Topics.Add(topic);
			context.SaveChanges();
		}
	}
}