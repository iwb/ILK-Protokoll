using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace ILK_Protokoll.util
{
	public static class HtmlHelperMethods
	{
		private static readonly Regex urlRegEx =
			new Regex(
				@"(?<!="")((http|ftp|https|file):\/\/[\d\w\-_]+(\.[\w\-_]+)*([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)");

		private static readonly Regex quotedUrlRegEx =
			new Regex(
				@"(?<!=)([""']|&quot;|&#39;)((http|ftp|https|file):\/\/[\d\w\-_]+(\.[\w\-_]+)*([\w\-\.,@?^=%&amp;:/~\+# ])*)\1");

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

		public static MvcHtmlString RenderHtmlAttributes<TModel>(
			this HtmlHelper<TModel> htmlHelper, object htmlAttributes)
		{
			var attributesDictionary = new RouteValueDictionary(htmlAttributes);

			return MvcHtmlString.Create(String.Join(" ",
				attributesDictionary.Keys.Select(
					key => String.Format("{0}=\"{1}\"", key,
						htmlHelper.Encode(attributesDictionary[key])))));
		}

		/// <summary>
		///    Zeigt den Text so an, dass URLs verlinkt werden.
		/// </summary>
		public static MvcHtmlString DisplayWithLinksFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			string templateName = null)
		{
			var encodedHTML = htmlHelper.DisplayFor(expression, templateName);
			return MvcHtmlString.Create(ReplaceUrlsWithLinks(encodedHTML.ToHtmlString()));
		}

		public static MvcHtmlString TextEditorFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			string id)
		{
            var name = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToString());
            return htmlHelper.EditorFor(expression, new {htmlAttributes = new {@class = "form-control", id, placeholder = name}});
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

		public static MvcHtmlString ToHex(this Color c)
		{
			return new MvcHtmlString(ColorTranslator.ToHtml(c));
		}
	}
}