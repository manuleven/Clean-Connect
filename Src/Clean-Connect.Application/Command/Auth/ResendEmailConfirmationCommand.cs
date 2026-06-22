using Clean_Connect.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.Auth
{
    public record ResendEmailConfirmationCommand(string Email) : IRequest<bool>;

    public class ResendEmailConfirmationCommandHandler
    : IRequestHandler<ResendEmailConfirmationCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResendEmailConfirmationCommandHandler> _logger;

        public ResendEmailConfirmationCommandHandler(
            UserManager<ApplicationUser> userManager,
            IMediator mediator,
            IConfiguration configuration,
            ILogger<ResendEmailConfirmationCommandHandler> logger)
        {
            _userManager = userManager;
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("Resend failed - user not found: {Email}", request.Email);
                return false;
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Resend skipped - email already confirmed: {Email}", request.Email);
                return false;
            }

            // 🔒 cooldown check
            var cooldownMinutes = 2;

            if (user.EmailConfirmationSentAt != null &&
                user.EmailConfirmationSentAt.Value.AddMinutes(cooldownMinutes) > DateTime.UtcNow)
            {
                _logger.LogInformation("Resend blocked due to cooldown: {Email}", request.Email);
                return false;
            }

            // 🔑 generate new token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var baseUrl = _configuration["App:ClientUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new Exception("App:ClientUrl is missing");

            var encodedToken = Uri.EscapeDataString(token);

            var callbackUrl =
                $"{baseUrl.TrimEnd('/')}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            // 📧 send email
            var emailMessage = new EmailSenderCommand(
                ToEmail: user.Email!,
                Subject: "Confirm your Clean Connect account",
                Body: $@"
                <h2>Verify Your Email</h2>
                <p>Click below to confirm your account:</p>

                <a href='{callbackUrl}'
                   style='padding:10px 18px;background:#28a745;
                   color:#fff;text-decoration:none;border-radius:6px;'>
                   Confirm Email
                </a>"
            );

            await _mediator.Send(emailMessage, cancellationToken);

            // 🕒 update timestamp
            user.MarkEmailConfirmationSent();
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Verification email resent to {Email}", request.Email);

            return true;
        }
    }
}
