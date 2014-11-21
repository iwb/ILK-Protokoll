using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ILK_Protokoll.Areas.Administration.Models;

namespace ILK_Protokoll.Models
{
	public class User : IEquatable<User>
	{
		public User()
		{
// ReSharper disable DoNotCallOverridableMethodsInConstructor
			SessionTypes = new List<SessionType>();
			UnreadTopics = new List<UnreadState>();
			IsActive = false;
			Settings = new UserSettings();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
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

		public string EmailName
		{
			get
			{
				var names = LongName.Split(',');
				if (names.Length < 2)
					return "";

				if (names[0].EndsWith(".RMV"))
					names[0] = names[0].Substring(0, names[0].Length - 4);

				return string.Join(" ", names.Reverse()).Trim();
			}
		}

		[Display(Name = "E-Mail Adresse")]
		[DataType(DataType.EmailAddress)]
		public string EmailAddress { get; set; }

		[Display(Name = "Aktiv")]
		public bool IsActive { get; set; }

		[InverseProperty("Attendees")]
		public virtual ICollection<SessionType> SessionTypes { get; set; }

		public virtual UserSettings Settings { get; set; }

		[Display(Name = "Push-Benachrichtigungen")]
		public virtual ICollection<PushNotification> PushNotifications { get; set; }

		public virtual ICollection<UnreadState> UnreadTopics { get; set; }

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
}