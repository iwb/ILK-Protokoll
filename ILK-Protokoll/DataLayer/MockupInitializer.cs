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
			var topic = new Topic() {
				Owner = new User("hz"),
				Title = "Mehr Kekse",
				Proposal = " Zukünftige Dissertationsthemen werden nur zugelassen, falls ein Zusammenhang mit Keksen deutlich erkennbar ist.",
				Description = "Das iwb sollte mehr Kekse backen.",
				Priority = Priority.High
			};
			topic.AuditorList.Add(new User("sw"));

			var sessiont = new SessionType() {ID = 1, Name = "Brotzeit"};
			topic.SessionType = sessiont;

			context.Topics.Add(topic);
			context.SessionTypes.Add(sessiont);
			context.SaveChanges();
		}
	}
}