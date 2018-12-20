using Mvc.Mailer;

namespace Submitter.Mailers
{ 
    public interface IMailer
    {
			MvcMailMessage ThankYou(string to, string subject);
	}
}