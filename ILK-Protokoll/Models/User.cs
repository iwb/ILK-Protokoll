using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ILK_Protokoll.Models
{
	public class User : IEquatable<User>
	{
		public User()
		{
			SessionTypes = new List<SessionType>();
		}

		public User(string name)
			: this()
		{
			Name = name;
		}

		public int ID { get; set; }
		public string Name { get; set; }


		[InverseProperty("Attendees")]
		public virtual ICollection<SessionType> SessionTypes { get; private set; }

		#region Equals() etc.

		public bool Equals(User other)
		{
			if ((object)other == null)
				return false;
			else
			{
				return Name == other.Name;
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as User);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator ==(User a, User b)
		{
			if (ReferenceEquals(a, b))
				return true;
			else if ((object)a == null || (object)b == null)
				return false;
			else
				return a.Name == b.Name;
		}

		public static bool operator !=(User a, User b)
		{
			return !(a == b);
		}
		#endregion
	}
}