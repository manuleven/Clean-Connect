using Clean_Connect.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.ApplicationUserCommand
{
    public record ResetPasswordCommand(string Email, string Token, string Password) : IRequest<bool>
    {
        public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
        {
            public ResetPasswordValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Invalid email address.");
                RuleFor(x => x.Token)
                    .NotEmpty().WithMessage("Token is required.");
                RuleFor(x => x.Password)
               .NotEmpty()
               .WithMessage("Password is required")
               .Length(8, 100)
               .WithMessage("Password must be between 8-100 characters")
               .Matches("[A-Z]")
               .WithMessage("Password must contain at least one uppercase letter")
               .Matches("[a-z]")
               .WithMessage("Password must contain at least one lowercase letter")
               .Matches("[0-9]")
               .WithMessage("Password must contain at least one digit")
               .Matches("[^a-zA-Z0-9]")
               .WithMessage("Password must contain at least one special character");
            }
        }

        public class ResetPasswordCommandHandler: IRequestHandler<ResetPasswordCommand, bool>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<bool> Handle(ResetPasswordCommand request,CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    return false;

                var result = await _userManager.ResetPasswordAsync(
                    user,
                    request.Token,
                    request.Password);

                return result.Succeeded;
            }
        }
    }
}
