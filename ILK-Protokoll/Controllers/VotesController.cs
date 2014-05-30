using ILK_Protokoll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.Controllers
{
	public class VotesController : BaseController
	{

		public ActionResult RegisterVote(int TopicID, VoteKind vote)
		{

			return View();
		}
	}
}