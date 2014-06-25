using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace ILK_Protokoll.util
{
	public static class HelperMethods
	{
		public static string GetDescription(this Enum value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());

			var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

			return attribute == null ? value.ToString() : attribute.Description;
		}

		public static string DisplayName(this Enum value)
		{
			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

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

		public static string Shorten(this string str, int maxLength)
		{
			if (str.Length > maxLength)
				return str.Substring(0, maxLength - 1) + "…";
			else
				return str;
		}

		public static MvcHtmlString DisplayColumnNameFor<TModel, TClass, TProperty>(
			this HtmlHelper<TModel> helper, IEnumerable<TClass> model,
			Expression<Func<TClass, TProperty>> expression)
		{
			var name = ExpressionHelper.GetExpressionText(expression);
			name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(
				() => Activator.CreateInstance<TClass>(), typeof(TClass), name);

			return new MvcHtmlString(metadata.DisplayName);
		}
	}
}