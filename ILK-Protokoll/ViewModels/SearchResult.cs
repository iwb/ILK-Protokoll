using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ILK_Protokoll.Models;
using ILK_Protokoll.util;

namespace ILK_Protokoll.ViewModels
{
	public class SearchResult : IEquatable<SearchResult>
	{
		public SearchResult()
		{
			Hits = new List<Hit>();
			Tags = new List<Tag>();
		}

		public SearchResult(string hittext) : this()
		{
			Hits.Add(new Hit(hittext));
		}

		public SearchResult(string hitname, string hittext)
			: this()
		{
			Hits.Add(new Hit(hitname, hittext));
		}

		public int ID { get; set; }

		public float Score { get; set; }

		public string EntityType { get; set; }

		public string Title { get; set; }

		public ICollection<Hit> Hits { get; set; }

		public string ActionURL { get; set; }

		public DateTime? Timestamp { get; set; }

		public IEnumerable<Tag> Tags { get; set; }

		public bool Equals(SearchResult other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return ID == other.ID && string.Equals(Title, other.Title) && string.Equals(EntityType, other.EntityType);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			var other = obj as SearchResult;
			return other != null && Equals(other);
		}

		public override int GetHashCode()
		{
			return ID;
		}

		public static bool operator ==(SearchResult left, SearchResult right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SearchResult left, SearchResult right)
		{
			return !Equals(left, right);
		}
	}

	public struct Hit
	{
		public readonly string Property;
		public readonly string Text;

		public Hit(string text) : this()
		{
			Property = "";
			Text = text;
		}

		public Hit(string property, string text)
			: this()
		{
			Property = property;
			Text = text;
		}

		public static Hit FromProperty<T>(T item, Expression<Func<T, string>> propertyExpression)
			where T : class
		{
			Expression converted = Expression.Convert(propertyExpression.Body, typeof(object));
			var expression = Expression.Lambda<Func<T, object>>(converted, propertyExpression.Parameters);

			var func = propertyExpression.Compile();
			return FromProperty(expression, func(item));
		}

		public static Hit FromProperty<T>(Expression<Func<T, object>> propertyExpression, string text)
			where T : class
		{
			var name = AttributeHelperMethods.GetPropertyDisplayName(propertyExpression);
			return new Hit(name, text);
		}
	}
}