using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ILK_Protokoll.Areas.Administration.Models;

namespace ILK_Protokoll.Models
{
	public class User : IEquatable<User>
	{
		public User()
		{
			SessionTypes = new List<SessionType>();
			IsActive = false;
		}

		public User(string name)
			: this()
		{
			ShortName = name;
		}

		public int ID { get; set; }

		[Required]
		public virtual Guid Guid { get; set; }

		[Display(Name = "Kürzel")]
		[Required]
		public string ShortName { get; set; }

		[Display(Name = "Name")]
		public string LongName { get; set; }

		[Display(Name = "E-Mail Adresse")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }

		[Display(Name = "Aktiv")]
		public bool IsActive { get; set; }

		[InverseProperty("Attendees")]
		public virtual ICollection<SessionType> SessionTypes { get; private set; }

		#region Equals() etc.

		public bool Equals(User other)
		{
			if ((object)other == null)
				return false;
			else
			{
				return ShortName == other.ShortName;
			}
		}

		public override string ToString()
		{
			return ShortName;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as User);
		}

		public override int GetHashCode()
		{
			return ShortName.GetHashCode();
		}

		public static bool operator ==(User a, User b)
		{
			if (ReferenceEquals(a, b))
				return true;
			else if ((object)a == null || (object)b == null)
				return false;
			else
				return a.ShortName == b.ShortName;
		}

		public static bool operator !=(User a, User b)
		{
			return !(a == b);
		}

		#endregion
	}
}