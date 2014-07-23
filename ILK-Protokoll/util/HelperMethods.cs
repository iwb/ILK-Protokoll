using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TuesPechkin;

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

		public static IEnumerable<Tuple<T, T>> Pairwise<T>(this IEnumerable<T> source)
		{
			using (var it = source.GetEnumerator())
			{
				if (!it.MoveNext())
					yield break;

				var previous = it.Current;

				while (it.MoveNext())
					yield return Tuple.Create(previous, previous = it.Current);
			}
		}

		public static string Shorten(this string str, int maxLength)
		{
			var stripped = Regex.Replace(str, @"\s+", " ");
			if (stripped.Length > maxLength)
				return stripped.Substring(0, maxLength - 1) + "…";
			else
				return stripped;
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

		public static string RenderViewAsString(ControllerContext controllerContext, string viewName, object model)
		{
			// create a string writer to receive the HTML code
			StringWriter stringWriter = new StringWriter();
			// get the view to render
			ViewEngineResult viewResult = ViewEngines.Engines.FindView(controllerContext, viewName, null);
			// create a context to render a view based on a model
			ViewContext viewContext = new ViewContext(
				controllerContext,
				viewResult.View,
				new ViewDataDictionary(model),
				new TempDataDictionary(),
				stringWriter);

			// render the view to a HTML code
			viewResult.View.Render(viewContext, stringWriter);

			var htmlstring = stringWriter.ToString();
			var localURL = controllerContext.HttpContext.Server.MapPath("~").Replace('\\', '/');

			var Request = controllerContext.RequestContext.HttpContext.Request;
			string baseURL = Request.Url.Scheme + "://" + Request.Url.Authority +
			                 Request.ApplicationPath.TrimEnd('/') + "/";


			htmlstring = Regex.Replace(htmlstring, "(img src|script src|link href)(=\")/([\\w./_-]+)\"",
				"$1$2file:///" + localURL + "$3\"");
			htmlstring = Regex.Replace(htmlstring, "(src|href)(=\")/([\\w./_-]+)\"", "$1$2" + baseURL + "$3\"");

			// return the HTML code
			return htmlstring;
		}

		public static byte[] ConvertHTMLToPDF(string sourceHTML)
		{
			// create a new document with your desired configuration
			var document = new HtmlToPdfDocument
			{
				GlobalSettings =
				{
					ProduceOutline = true,
					DocumentTitle = "Sitzungsprotokoll",
					DPI = 600,
					ImageDPI = 1200,
					ImageQuality = 100,
					Margins =
					{
						All = 1,
						Unit = Unit.Centimeters
					}
				},
				Objects =
				{
					new ObjectSettings()
					{
						HtmlText = sourceHTML,
						WebSettings =
						{
							EnableJavascript = true,
							PrintBackground = true,
							PrintMediaType = false,
							EnableIntelligentShrinking = false
						}
					}
				}
			};

			// create converter
			IPechkin converter = Factory.Create();

			// convert document
			return converter.Convert(document);
		}
	}
}