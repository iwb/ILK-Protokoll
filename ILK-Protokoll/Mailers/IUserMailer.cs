using Mvc.Mailer;

namespace ILK_Protokoll.Mailers
{ 
    public interface IUserMailer
    {
			MvcMailMessage Welcome();
	}
}