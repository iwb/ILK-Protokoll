using System;
using System.ComponentModel;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	public class BaseItem
	{
		protected BaseItem()
		{
			LastChanged = DateTime.Now;
			LockTime = new DateTime(2000, 1, 1);
		}

		public int ID { get; set; }

		/// <summary>
		/// Gibt an, wann dieser Eintrag zuletzt geändert wurde. Dadurch kann der Eintrag markiert werden, falls seit der letzten Sitzung eine Änderung vorgenommen wurde.
		/// </summary>
		public DateTime LastChanged { get; set; }

		/// <summary>
		///    Enthält den letzten Sperrzeitpunkt.
		/// </summary>
		public DateTime LockTime { get; set; }

		/// <summary>
		///    Enthält die ActiveSessionID, die diesen Einträgt gesperrt hält. NULL, wen kein Lock besteht.
		/// </summary>
		public int? LockSessionID { get; set; }

		/// <summary>
		///    Enthält eine GUID, falls eine vergeben ist.
		/// </summary>
		public Guid? GUID { get; set; }
	}
}