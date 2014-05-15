using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class User
	{
		public User(string name = "")
		{
			Name = name;
		}
		public int ID { get; set; }
		public string Name { get; set; }
	}
}