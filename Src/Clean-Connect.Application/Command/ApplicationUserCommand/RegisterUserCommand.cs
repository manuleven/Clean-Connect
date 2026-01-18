using Clean_Connect.Application.Command.Auth;
using Clean_Connect.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.ApplicationUserCommand
{
    public record RegisterUserCommand(string Email, string Password) : IRequest<Guid>;

    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
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

    public class RegisterUserCommandHandler(UserManager<ApplicationUser> user, ILogger<RegisterUserCommandHandler> logger, IConfiguration _configuration, IMediator mediator) : IRequestHandler<RegisterUserCommand, Guid>
    {

        ////private readonly IConfiguration _configuration;

        //public RegisterUserCommandHandler(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}


        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {

            var email = request.Email.Trim().ToLower();
            var checkExistingUser = await user.FindByEmailAsync(email);
            if (checkExistingUser != null)
            {
                logger.LogWarning("User with email {Email} already exists", email);

                throw new ValidationException("User with this email already exists");

            }

            var newUser = ApplicationUser.Create(email);

            var result = await user.CreateAsync(newUser, request.Password);



            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                logger.LogError("Failed to create user with email {Email}. Errors: {Errors}", email, errors);
                throw new ValidationException($"Failed to create user: {errors}");
            }

            var token = await user.GenerateEmailConfirmationTokenAsync(newUser);

            var confirmationLink = $"{_configuration["App:ClientUrl"]}/confirm-email?userId={newUser.Id}&token={Uri.EscapeDataString(token)}";

            var emailMessage = new EmailSenderCommand(
                ToEmail: newUser.Email,
                Subject: "Confirm your email",
                Body: $"Please confirm your email by clicking on the following link: <a href='{confirmationLink}'>Confirm Email</a>");

            await mediator.Send(emailMessage, cancellationToken);


            logger.LogInformation("User with email {Email} created successfully", request.Email);
            return newUser.Id;

        }

    }
}
