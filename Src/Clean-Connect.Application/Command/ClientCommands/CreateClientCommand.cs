using Clean_Connect.Application.Command.Services;
using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Clean_Connect.Domain.Value_Objects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Clean_Connect.Application.Command.ClientCommands
{
    public record CreateClientCommand(string FirstName, string LastName, string Email, double Latitude, double Longitude, string Contact,  string Gender, string State, DateTime Dob, string? CreatedBy = null) : IRequest<bool>;

    public class CreateClientValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .Length(2, 50)
                .WithMessage("First name must be between 2-50 characters");


            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .Length(2, 50)
                .WithMessage("Last name must be between 2-50 characters");

            

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(g => Enum.TryParse<Gender>(g, true, out _))
                .WithMessage("Gender must be Male or Female");

            RuleFor(x => x.Latitude)
               .InclusiveBetween(4.0, 14.0)
               .WithMessage("Latitude must be between 4.0 and 14.0.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(2.5, 15.5)
                .WithMessage("Longitude must be between 2.5 and 15.5.");


            RuleFor(x => x.Email)
                 .NotEmpty()
                 .WithMessage("Email is required")
                 .EmailAddress()
                 .WithMessage("Email is invalid")
                 .Length(10, 100)
                 .WithMessage("Email must be between 10-100 characters");

            RuleFor(x => x.Contact)
                .NotEmpty()
                .WithMessage("Contact is required")
                .Length(11, 15)
                .WithMessage("Contact must be between 11-15 characters");

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage("State is required")
                .Length(3, 20)
                .WithMessage("State name must be 3-20 characters");

            RuleFor(x => x.Dob)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Date of birth cannot be in the future");
        }


    }

    public class CreateClientCommandHandler(IUnitOfWork repo, GeocodingService geocodingService, ILogger<CreateClientCommandHandler>logger) : IRequestHandler<CreateClientCommand, bool>
    {
        public async Task<bool> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {

            if (!Enum.TryParse<Gender>(request.Gender, true, out var gender))
            {
                throw new ValidationException("Invalid gender value");
            }


            var EmailExists = await repo.Clients.GetByEmail(request.Email.Trim(), cancellationToken);
            if (EmailExists != null)
            {
                logger.LogError("Client with email {Email} already exists", request.Email);
                throw new DuplicateNameException($"Client with email {request.Email} already exists");
            }



            var fullname = FullName.Create(request.FirstName, request.LastName);
            var email = Email.Create(request.Email);
            var location = Location.Create(request.Latitude, request.Longitude);
            var discoverAddress = await geocodingService.GetAddressAsync(request.Latitude, request.Longitude);
            if (!discoverAddress.Contains("Nigeria"))
                throw new Exception("Location must be in Nigeria.");
            var address = Address.Create(discoverAddress);
            var contact = PhoneNumber.Create(request.Contact);
            var client = Client.Create(
                fullname,
                address,
                email,
                location,
                gender,
                contact,
                request.State,
                request.Dob,
                request.CreatedBy);

            await repo.Clients.CreateClient(client, cancellationToken);
            var result = await repo.SaveChangesAsync(cancellationToken) > 0;

            logger.LogInformation("Client created successfully with ID: {ClientId}", client.Id);
            return result;
        }
    }
}

