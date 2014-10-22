using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ILK_Protokoll.util
{
	public static class AttributeHelperMethods
	{
		/// <summary>
		/// Liefert das Attribut des angegebenen Typs zurück, das mit dem Enumwert assizoiert ist. 
		/// </summary>
		/// <typeparam name="T">Gesuchter Attributtyp</typeparam>
		/// <param name="value">Enumwert, dessen Attribut ermittelt werden soll.</param>
		/// <returns>Die Instanz des Attributtyps, die dem Attribut am Enum entspricht.</returns>
		/// <exception cref="System.Reflection.AmbiguousMatchException">Es gibt mehrere Attribute dieses Typs.</exception>
		private static T GetAttribute<T>(this Enum value)
			where T : Attribute
		{
			var field = value.GetType().GetField(value.ToString());
			return Attribute.GetCustomAttribute(field, typeof(T)) as T;
		}

		private static TAttr GetAttribute<TAttr, TTarget>()
		where TAttr : Attribute
		{
			return typeof(TTarget).GetCustomAttribute(typeof(TAttr), true) as TAttr;
		}

		/// <summary>
		/// Liefert die Beschreibung (DescriptionAttribute.Description) des Enumwerts.
		/// </summary>
		/// <param name="value">Ein gültiger Enumwert.</param>
		/// <returns>Die Beschreibung falls vorhanden, ansonsten value.ToString().</returns>
		public static string GetDescription(this Enum value)
		{
			var attribute = value.GetAttribute<DescriptionAttribute>();
			return attribute == null ? value.ToString() : attribute.Description;
		}

		/// <summary>
		/// Ermittelt den Anzeigenamen (DisplayAttribute.Name) des Enumwerts.
		/// </summary>
		/// <param name="value">Ein gültiger Enumwert.</param>
		/// <returns>Den Anzeigenamen falls vorhanden, ansonsten value.ToString().</returns>
		public static string DisplayName(this Enum value)
		{
			var attribute = value.GetAttribute<DisplayAttribute>();
			return attribute == null ? value.ToString() : attribute.Name;
		}

		/// <summary>
		/// Ermittelt die Anzeigereihenfolge (DisplayAttribute.Order) des Enumwerts.
		/// </summary>
		/// <param name="value">Ein gültiger Enumwert.</param>
		/// <returns>Die Reiehnfolge falls vorhanden, ansonsten den Ordinalwert von value.</returns>
		public static int DisplayOrder(this Enum value)
		{
			var attribute = value.GetAttribute<DisplayAttribute>();
			return attribute == null ? (int)Convert.ChangeType(value, typeof(int)) : attribute.Order;
		}

		public static string GetDisplayName<TClass>(this HtmlHelper htmlHelper)
		{
			var attr = GetAttribute<DisplayNameAttribute, TClass>();
			return attr != null ? attr.DisplayName : typeof(TClass).Name;
		}
	}
}