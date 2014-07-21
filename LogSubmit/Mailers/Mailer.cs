using Mvc.Mailer;

namespace LogSubmit.Mailers
{ 
    public class Mailer : MailerBase, IMailer 	
	{
		public Mailer()
		{
			MasterName="_Layout";
		}
		
		public virtual MvcMailMessage ThankYou(string to)
		{
			//ViewBag.Data = someObject;
			return Populate(x =>
			{
				x.Subject = "Log - Cupa Timisului 2013";
				x.ViewName = "ThankYou";
				x.To.Add(to);
			});
		}
 	}
}