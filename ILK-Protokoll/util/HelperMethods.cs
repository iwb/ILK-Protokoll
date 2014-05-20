using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ILK_Protokoll.util
{
	public static class HelperMethods
	{
		public static string GetDescription(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());

			var attribute = Attribute.GetCustomAttribute(field, typeof (DescriptionAttribute)) as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}

		public static string DisplayName(this Enum value)
		{
			var fieldInfo = value.GetType().GetField(value.ToString());

			var descriptionAttributes = fieldInfo.GetCustomAttributes(
				 typeof(DisplayAttribute), false) as DisplayAttribute[];

			if (descriptionAttributes == null)
				return string.Empty;
			else
				return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
		}
		public static IEnumerable<T> ToEnumerable<T>(this T item)
		{
			yield return item;
		}
	}
}