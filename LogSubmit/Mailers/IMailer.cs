using Mvc.Mailer;

namespace LogSubmit.Mailers
{ 
    public interface IMailer
    {
			MvcMailMessage ThankYou(string to, string subject);
	}
}