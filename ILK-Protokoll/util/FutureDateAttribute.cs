using System;
using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.util
{
	public class FutureDateAttribute : ValidationAttribute
	{
		public FutureDateAttribute()
		{
			ErrorMessage = "Das Datum muss in der Zukunft liegen.";
		}

		public override bool IsValid(object value)
		{
			return value != null && (DateTime)value >= DateTime.Today;
		}
	}
}