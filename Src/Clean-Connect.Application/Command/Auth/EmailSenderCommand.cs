using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.Auth
{
    public record EmailSenderCommand(string ToEmail, string Body, string Subject) : IRequest<Unit>;

    public class EmailSenderHandler : IRequestHandler<EmailSenderCommand, Unit>
    {
        private readonly IConfiguration _configuration; 

        public EmailSenderHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Unit> Handle(EmailSenderCommand request, CancellationToken cancellationToken)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = int.Parse(_configuration["Email:Smtp:Port"]),
                Credentials = new NetworkCredential(
                    _configuration["Email:Smtp:Username"],
                    _configuration["Email:Smtp:Password"]
                    
                    ),

                EnableSsl = true
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(_configuration["Email:Smtp:From"]),
                Body = request.Body,
                Subject = request.Subject,
                IsBodyHtml = true
            };

            mailMessage.To.Add(request.ToEmail);

            await smtpClient.SendMailAsync(mailMessage);

            return Unit.Value;
        }
    }
   
}
