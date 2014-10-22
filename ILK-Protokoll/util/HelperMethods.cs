using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using JetBrains.Annotations;
using TuesPechkin;

namespace ILK_Protokoll.util
{
	public static class HelperMethods
	{
		/// <summary>
		///    Verpackt den angegebenen Wert in eine Enumeration mit einem Element.
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
		///    Liefert zu einer Enumeration alle Paare zurück. Eine Enumeration mit n Elementen hat genau n-1 Paare.
		///    Die Quelle wird nur einmal durchlaufen. Für jedes Paar wird ein neues Tupel generiert.
		///    Item1 ist stets das Element, dass in der Quelle zuerst vorkommt.
		/// </summary>
		/// <param name="source">Die Quelle, die paarweise enumeriert werden soll.</param>
		/// <returns>
		///    Eine Enumeration mit n-1 überschneidenden Tupeln. Gibt eine leere Enumeration zurück, wenn die Quelle aus
		///    weniger als zwei Elmenten besteht.
		/// </returns>
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
		///    Kürzt den String auf eine Länge ein und versieht das Ergebnis mit einer Ellipse als Endzeichen. Wenn möglich, wid an
		///    einer Wortgrenze abgeschnitten. Ist <paramref name="str" /> nach Entfernung von mehrfachen Leerraumzeichen kürzer
		///    als <paramref name="maxLength" /> wird der String (verändert) zurückgegeben.
		/// </summary>
		/// <param name="str">Der Eingabestring</param>
		/// <param name="maxLength">Die maximale Länge (in Zeichen) des Ausgabestrings</param>
		/// <returns>Ein String, der maximal <paramref name="maxLength" /> Zeichen lang ist.</returns>
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

			var request = controllerContext.RequestContext.HttpContext.Request;

			if (request == null || request.Url == null || request.ApplicationPath == null)
				throw new InvalidOperationException();

			string baseURL = request.Url.Scheme + "://" + request.Url.Authority +
			                 request.ApplicationPath.TrimEnd('/') + "/";

			htmlstring = Regex.Replace(htmlstring, "(img src|script src|link href)(=\")/([\\w./_-]+)\"",
				"$1$2file:///" + localURL + "$3\"");
			htmlstring = Regex.Replace(htmlstring, "(src|href)(=\")/([\\w./_-]+)\"", "$1$2" + baseURL + "$3\"");

			// return the HTML code
			return htmlstring;
		}

		public static byte[] ConvertHTMLToPDF(string sourceHTML)
		{
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
			IPechkin converter = Factory.Create();
			return converter.Convert(document);
		}
	}
}