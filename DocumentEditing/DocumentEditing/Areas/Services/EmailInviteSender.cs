using DocumentEditing.Areas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DocumentEditing.Areas.Services
{

	public class EmailInviteSender : IInviteSender
	{
		public EmailInviteSender(IHttpContextAccessor httpContext)
		{
			_httpContext = httpContext;			
		}

		private IHttpContextAccessor _httpContext { get; }

		[Authorize]
		public async Task SendInvite(string userEmail, string userPassword, string projectName)
		{
			//data of mail.ru account
			//sender email - example_inviter@mail.ru
			//sender password - 123567Zz

			//create MailMessage
			MailAddress sender = new MailAddress("example_inviter@mail.ru", "Ivan");
			MailAddress recipient = new MailAddress(userEmail);
			MailMessage msg = new MailMessage(sender, recipient);

			//topic of letter
			msg.Subject = "Project invite";

			//link to site's URL			
			var request = _httpContext.HttpContext.Request;
			var projectLink = $"{request.Scheme}://{request.Host.ToUriComponent()}";

			//body of message
			msg.Body = $"<h4>You were invited to project - {projectName}</h4>" +
						$"<a href=\"{ projectLink}\">Link to site</a> " +
						$"<p>Your log in data:</p>" +
						$"Email: {userEmail}<br/>" +
						$"Password: {userPassword}";

			msg.IsBodyHtml = true;

			//create SMTP client to sent message with right host data
			SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);//587
			
			smtp.UseDefaultCredentials = false;

			//sender's personal data
			var credentials = new NetworkCredential("example_inviter@mail.ru", "123567Zz");
			smtp.Credentials = credentials;
			smtp.EnableSsl = true;

			await smtp.SendMailAsync(msg);			
		}
	}
}
