using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace IdentityNetCore.Service;

public class SmtpEmailSender : IEmailSender
{
    private readonly IOptions<SmtpOptions> options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        this.options = options;
    }

    public async Task SendEmailAsync(string fromAddress, string toAddress, string subject, string message)
    {
        var mailMessage = new MailMessage(fromAddress, toAddress, subject, message);
        using (var client = new SmtpClient(options.Value.Host, options.Value.Port)
               {
                   Credentials = new NetworkCredential(options.Value.Username, options.Value.Password)
               })
        {
            await client.SendMailAsync(mailMessage);
        }
    }
}