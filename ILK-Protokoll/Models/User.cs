using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		public User(string name, Guid guid)
			: this(name)
		{
			Guid = guid;
		}

		public int ID { get; set; }

		[Required]
		[Index("guid_index", IsUnique = true)]
		public Guid Guid { get; set; }

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
		public virtual ICollection<SessionType> SessionTypes { get; set; }

		[DisplayName("Farbschema")]
		public ColorScheme ColorScheme { get; set; }

		#region Equals() etc.

		public bool Equals(User other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return Guid.Equals(other.Guid);
		}

		public override string ToString()
		{
			return ShortName;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(User)) return false;
			return Equals((User)obj);
		}

		public override int GetHashCode()
		{
			return ShortName.GetHashCode();
		}

		#endregion
	}

	public enum ColorScheme
	{
		[Display(Name = "iwb Blau")] iwb,
		[Display(Name = "RMV grün")] RMV
	}
}