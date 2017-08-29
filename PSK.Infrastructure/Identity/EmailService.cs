using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Net;

namespace PSK.Infrastructure.Identity
{
	public class EmailService : IIdentityMessageService
	{
        public Task SendAsync(IdentityMessage message)
		{
            var fromAddress = new MailAddress("psk.pasroviui@gmail.com", "From Name");
            var toAddress = new MailAddress(message.Destination, "To Name");
            const string fromPassword = "pasroviui123";
            string subject = message.Subject;
            string body = message.Body;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var emailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(emailMessage);
            }

            return Task.FromResult(0);
		}
	}
}