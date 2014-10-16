using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using JetBrains.Annotations;
using TuesPechkin;

namespace ILK_Protokoll.util
{
	public static class HelperMethods
	{
		private static readonly Regex urlRegEx =
			new Regex(
				@"(?<!="")((http|ftp|https|file):\/\/[\d\w\-_]+(\.[\w\-_]+)*([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

		private static readonly Regex quotedUrlRegEx =
			new Regex(
				@"(?<!=)([""']|&quot;|&#39;)((http|ftp|https|file):\/\/[\d\w\-_]+(\.[\w\-_]+)*([\w\-\.,@?^=%&amp;:/~\+# ])*)\1");

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

		/// <summary>
		/// Verpackt den angegebenen Wert in eine Enumeration mit einem Element.
		/// </summary>
		/// <typeparam name="T">Ein beliebiger Typ.</typeparam>
		/// <param name="item">Der Wert, der verpackt werden soll.</param>
		/// <returns>Eine Enumeration, die genau einen Wert enthält.</returns>
		[NotNull]
		public static IEnumerable<T> ToEnumerable<T>(this T item)
		{
			yield return item;
		}

		/// <summary>
		/// Liefert zu einer Enumeration alle Paare zurück. Eine Enumeration mit n Elementen hat genau n-1 Paare.
		/// Die Quelle wird nur einmal durchlaufen. Für jedes Paar wird ein neues Tupel generiert.
		/// Item1 ist stets das Element, dass in der Quelle zuerst vorkommt.
		/// </summary>
		/// <param name="source">Die Quelle, die paarweise enumeriert werden soll.</param>
		/// <returns>Eine Enumeration mit n-1 überschneidenden Tupeln. Gibt eine leere Enumeration zurück, wenn die Quelle aus weniger als zwei ELmenten besteht.</returns>
		[NotNull]
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

		/// <summary>
		/// Kürzt den String auf eine Länge ein und versieht das Ergebnis mit einer Ellipse als Endzeichen. Wenn möglich, wid an einer Wortgrenze abgeschnitten. Ist <paramref name="str"/> nach Entfernung von mehrfachen Leerraumzeichen kürzer als <paramref name="maxLength"/> wird der String (verändert) zurückgegeben.
		/// </summary>
		/// <param name="str">Der Eingabestring</param>
		/// <param name="maxLength">Die maximale Länge (in Zeichen) des Ausgabestrings</param>
		/// <returns>Ein String, der maximal <paramref name="maxLength"/> Zeichen lang ist.</returns>
		public static string Shorten(this string str, int maxLength)
		{
			var stripped = Regex.Replace(str.Trim(), @"\s+", " ");
			if (stripped.Length > maxLength)
			{
				var lastspace = stripped.LastIndexOf(' ', maxLength - 1, 12);
				if (lastspace >= 0)
					return stripped.Substring(0, lastspace) + "…";
				else
					return stripped.Substring(0, maxLength - 1) + "…";
			}
			else
				return stripped;
		}

		/// <summary>
		/// Zeigt den Text so an, dass URLs verlinkt werden.
		/// </summary>
		public static MvcHtmlString DisplayWithLinksFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			string templateName = null)
		{
			var encodedHTML = htmlHelper.DisplayFor(expression, templateName);
			return MvcHtmlString.Create(ReplaceUrlsWithLinks(encodedHTML.ToHtmlString()));
		}

		private static string ReplaceUrlsWithLinks(string input)
		{
			input = input.Replace(@"\\", @"file://").Replace('\\', '/');

			var result = quotedUrlRegEx.Replace(input, delegate(Match match)
			{
				string url = match.Groups[2].Value;
				return String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", Uri.EscapeUriString(url), ShortenURL(url));
			});
			return urlRegEx.Replace(result, delegate(Match match)
			{
				string url = match.ToString();
				return String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", Uri.EscapeUriString(url), ShortenURL(url));
			});
		}

		private static string ShortenURL(string url)
		{
			url = url.Substring(url.IndexOf("//", StringComparison.Ordinal) + 2);
			if (url.Length < 60)
				return url;
			var host = url.Substring(0, url.IndexOf("/", StringComparison.Ordinal));
			return host + "/&hellip;";
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

		public static MvcHtmlString GetDisplayName<TModel, TProperty>(
			this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression
			)
		{
			var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression));
			return MvcHtmlString.Create(value);
		}

		public static string GetDisplayName<TClass>(this HtmlHelper htmlHelper)
		{
			var attr = GetAttribute<DisplayNameAttribute, TClass>();
			return attr != null ? attr.DisplayName : typeof(TClass).Name;
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
					new ObjectSettings
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

		public static MvcHtmlString RenderHtmlAttributes<TModel>(
			this HtmlHelper<TModel> htmlHelper, object htmlAttributes)
		{
			var attributesDictionary = new RouteValueDictionary(htmlAttributes);

			return MvcHtmlString.Create(String.Join(" ",
				attributesDictionary.Keys.Select(
					key => String.Format("{0}=\"{1}\"", key,
						htmlHelper.Encode(attributesDictionary[key])))));
		}

		public static MvcHtmlString ToHex(this Color c)
		{
			return new MvcHtmlString(ColorTranslator.ToHtml(c));
		}
	}
}