using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.ClientCommands
{


    public record UpdateClientCommand(Guid ClientId, string FirstName, string LastName, string Address, string Contact, string Email, Gender Gender, string State, DateTime DateOfBirth, string? ModifiedBy = null) : IRequest<bool>;

    public class UpdateClientValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientValidator()
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

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required")
                .Length(10, 200)
                .WithMessage("Address must be between 10-200 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email is invalid")
                .Length(10, 100)
                .WithMessage("Email must be between 10-100 characters");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender is required")
                .IsInEnum();

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage("State is required")
                .Length(3, 20)
                .WithMessage("State must be between 3-20 characters");

            RuleFor(x => x.Contact)
                .NotEmpty()
                .WithMessage("Contact is required")
                .Length(11, 15)
                .WithMessage("Contact must be between 11-15 characters");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of Birth is required")
                .LessThan(DateTime.Now)
                .WithMessage("Date of Birth must be in the past");
        }
    }

    public class UpdateClientCommandHandler(IUnitOfWork repo, ILogger<UpdateClientCommandHandler> logger) : IRequestHandler<UpdateClientCommand, bool>
    {
        public async Task<bool> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await repo.Clients.GetClientById(request.ClientId, cancellationToken);
            if(client == null)
            {
                logger.LogError("Client with ID {ClientId} not found", request.ClientId);
                throw new KeyNotFoundException($"Client with ID {request.ClientId} not found");
            }

            var email = request.Email.Trim();

            var existingClient = await repo.Clients.GetByEmail(email, cancellationToken);

            if (existingClient != null && existingClient.Id != client.Id)
            {
                logger.LogWarning(
                    "Attempt to update client {ClientId} with duplicate email {Email}",
                    client.Id,
                    email
                );

                throw new InvalidOperationException($"Email {email} is already in use.");
            }


            client.UpdateName(request.FirstName, request.LastName, request.ModifiedBy);
            client.UpdateEmail(request.Email.Trim(), request.ModifiedBy);
            client.UpdateAddress(request.Address, request.ModifiedBy);
            client.UpdateContact(request.Contact, request.ModifiedBy);
            client.UpdateState(request.State, request.ModifiedBy);
            client.UpdateDateOfBirth(request.DateOfBirth, request.ModifiedBy);
            client.UpdateGender(request.Gender, request.ModifiedBy);

            await repo.Clients.UpdateClient(client, cancellationToken);
            await repo.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Client with ID {ClientId} updated successfully", request.ClientId);
            return true;

        }
    }
}
