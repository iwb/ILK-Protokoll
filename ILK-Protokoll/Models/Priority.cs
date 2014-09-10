using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Models
{
	public enum Priority
	{
		[Display(Name = "Niedrig")] Low,
		[Display(Name = "Normal")] Medium,
		[Display(Name = "Hoch")] High
	}
}