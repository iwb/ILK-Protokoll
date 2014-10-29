using System.ComponentModel.DataAnnotations;

namespace ILK_Protokoll.Areas.Session.Models.Lists
{
	public enum Approval
	{
		[Display(Name = "Ausstehend")] Pending,
		[Display(Name = "Genehmigt")] Approved,
		[Display(Name = "Abgelehnt")] Rejected
	}
}