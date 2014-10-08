using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Models
{
	public class UserSettings
	{
		public UserSettings()
		{
			ColorScheme = ColorScheme.iwb;
			ReportOccasions = SessionReportOccasions.Always;
		}

		[DisplayName("Farbschema")]
		public ColorScheme ColorScheme { get; set; }

		[DisplayName("E-Mail Protokolle")]
		public SessionReportOccasions ReportOccasions { get; set; }
	}

	public enum ColorScheme
	{
		[Display(Name = "iwb Blau")] iwb,
		[Display(Name = "RMV Grün")] RMV
	}

	public enum SessionReportOccasions
	{
		[Display(Name = "Immer")] Always,
		[Display(Name = "Bei Abwesenheit")] WhenAbsent,
		[Display(Name = "Niemals")] Never
	}
}