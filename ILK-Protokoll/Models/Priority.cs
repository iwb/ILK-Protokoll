using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Models
{
	public enum Priority
	{
		[Display(Name = "Niedrig")] Low,
		[Display(Name = "Mittel")] Medium,
		[Display(Name = "Hoch")] High
	}
}