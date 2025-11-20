using MerchShop.Models.DomainModels;
using System.Net;
using System.Net.Mail;

namespace MerchShop.Services
{
	public static class EmailService
	{
		public static void SendLowStockAlert(Inventory inventory)
		{
			// Use SmtpClient or any email library
			// Example:
			string fromEmail = "************@gmail.com";
			string password = "******************";
			var message = new MailMessage(fromEmail, fromEmail)
			{
				Subject = $"Low Stock Alert: {inventory.Merch.ItemName}",
				Body = $"The item '{inventory.Merch.ItemName}' is low in stock (Quantity: {inventory.Quantity})."
			};
			using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
			{
				smtpClient.EnableSsl = true;
				smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtpClient.UseDefaultCredentials = false;
				smtpClient.Credentials = new NetworkCredential(fromEmail, password);

				smtpClient.Send(message);
			}
		}
	}

}
