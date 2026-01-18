using Clean_Connect.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.ApplicationUserCommand
{
    public record LoginCommand(string Email, string Password, bool RememberMe) : IRequest<Guid>;

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email is invalid")
                .Length(10, 100)
                .WithMessage("Email must be between 10-100 characters");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .Length(8, 100)
                .WithMessage("Password must be between 8-100 characters");
        }
    }

    public class LoginCommandHandler(UserManager<ApplicationUser> user, SignInManager<ApplicationUser> signInManager, ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<LoginCommand, Guid>
    {
        public async Task<Guid> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var login = await signInManager.PasswordSignInAsync(request.Email, request.Password, request.RememberMe, lockoutOnFailure: false);
            if (!login.Succeeded)
            {
                logger.LogWarning("Login failed for email: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var appUser = await user.FindByEmailAsync(request.Email);
            if (appUser == null)
            {
                logger.LogError("User not found after successful login: {Email}", request.Email);
                throw new InvalidOperationException("User not found.");
            }

            logger.LogInformation("User logged in: {Email}", request.Email);
            return appUser.Id;

        }
    }


}
