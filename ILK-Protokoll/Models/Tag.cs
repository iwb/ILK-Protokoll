using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ILK_Protokoll.Models
{
	public class Tag
	{
		private Color _backgroundColor;

		public Tag()
		{
			BackgroundColor = Color.Firebrick;
// ReSharper disable DoNotCallOverridableMethodsInConstructor
			Topics = new List<TagTopic>();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public int ID { get; set; }

		[Required]
		[DisplayName("Name")]
		public string Name { get; set; }

		public Int32 BGColor
		{
			get { return BackgroundColor.ToArgb(); }
			set { BackgroundColor = Color.FromArgb(value); }
		}

		[NotMapped]
		[DisplayName("Farbe")]
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				FontColor = GetContrastColor(value);
			}
		}

		public Int32 TxtColor
		{
			get { return FontColor.ToArgb(); }
			set { FontColor = Color.FromArgb(value); }
		}

		[NotMapped]
		[DisplayName("Textfarbe")]
		public Color FontColor { get; set; }

		public virtual ICollection<TagTopic> Topics { get; set; }

		private Color GetContrastColor(Color c)
		{
			var brightness = (299 * c.R + 587 * c.G + 114 * c.B) / 1000;
			return brightness >= 128 ? Color.Black : Color.White;
		}
	}

	public class TagTopic
	{
		public int ID { get; set; }

		public int TopicID { get; set; }
		public virtual Topic Topic { get; set; }

		public int TagID { get; set; }
		public virtual Tag Tag { get; set; }
	}
}