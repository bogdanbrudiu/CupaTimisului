using Mvc.Mailer;

namespace LogSubmit.Mailers
{ 
    public class Mailer : MailerBase, IMailer 	
	{
		public Mailer()
		{
			MasterName="_Layout";
		}
		
		public virtual MvcMailMessage ThankYou(string to, string subject)
		{
			//ViewBag.Data = someObject;
			return Populate(x =>
			{
				x.Subject = subject;
				x.ViewName = "ThankYou";
				x.To.Add(to);
			});
		}
 	}
}