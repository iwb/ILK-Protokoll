using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ILK_Protokoll.Models
{
	public class Tag
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public Int32 RGBColor
		{
			get { return Color.ToArgb(); }
			set { Color = Color.FromArgb(value); }
		}

		[NotMapped]
		public Color Color { get; set; }

		public virtual ICollection<Topic> Topics { get; set; }
	}
}