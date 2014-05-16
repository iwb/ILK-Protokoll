using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ILK_Protokoll.Models
{
	public class User
	{
		public User() : this("") { }

		public User(string name)
		{
			Name = name;
		}
		public int ID { get; set; }
		public string Name { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}