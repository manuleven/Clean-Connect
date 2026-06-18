using Clean_Connect.Application.Command.Auth;
using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Services;
using Clean_Connect.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.ApplicationUserCommand
{
    public record LoginCommand(string Email, string Password, bool RememberMe) : IRequest<LoginResponse>;

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

    public class LoginCommandHandler(UserManager<ApplicationUser> user,  IMediator _mediator, SignInManager<ApplicationUser> signInManager, ILogger<RegisterUserCommandHandler> logger, IConfiguration configuration) : IRequestHandler<LoginCommand, LoginResponse>
    {
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user first so we can return meaningful (but not leaking) errors
            var appUser = await user.FindByEmailAsync(request.Email);
            if (appUser == null)
            {
                logger.LogWarning("Login failed for email: {Email} (invalid password)", request.Email);
                return new LoginResponse
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid email or password.",
                    UserId = Guid.Empty
                };
            }
      

            var login = await signInManager.PasswordSignInAsync(appUser,request.Password,request.RememberMe,lockoutOnFailure: false);
            if (!login.Succeeded)
            {
                logger.LogWarning("Login failed for email: {Email} (user not found)", request.Email);
                return new LoginResponse
                {
                    IsSuccessful = false,
                    ErrorMessage = "Invalid email or password.",
                    UserId = appUser.Id,
                    
                };
            }

            var token = await _mediator.Send(new JwtTokenCommand(appUser), cancellationToken);
            var roles = await user.GetRolesAsync(appUser);

            logger.LogInformation("User logged in: {Email}", request.Email);

            return new LoginResponse
            {
                UserId = appUser.Id,
                Email = appUser.Email ?? request.Email,
                Roles = roles.ToArray(),
                Token = token,
                IsSuccessful = true,
               
            };
        }
    }
}
