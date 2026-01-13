using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Clean_Connect.Domain.Value_Objects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.WorkerCommands
{
    public record UpdateWorkerCommand(Guid WorkerId, string FirstName, string LastName, string Address, string Contact, string Email, Gender Gender, string State, DateTime DateOfBirth, string? ModifiedBy = null) : IRequest<bool>;


    public class UpdateWorkerValidator : AbstractValidator<UpdateWorkerCommand>
    {
        public UpdateWorkerValidator()
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

            RuleFor(x => x.Gender)
                .NotEmpty()
                .WithMessage("Gender is required")
                .IsInEnum();

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage("Date of Birth is required")
                .LessThan(DateTime.Now)
                .WithMessage("Date of Birth must be in the past");
        }
    }

    public class UpdateWorkerHandler(IUnitOfWork repo, ILogger<UpdateWorkerHandler> logger) : IRequestHandler<UpdateWorkerCommand, bool>
    {
        public async Task<bool> Handle(UpdateWorkerCommand request, CancellationToken cancellationToken)
        {
            var worker = await repo.Workers.GetWorkerById(request.WorkerId, cancellationToken);
            if (worker == null)
            {
                logger.LogError("Worker with ID {WorkerId} not found", request.WorkerId);
                throw new KeyNotFoundException($"Worker with ID {request.WorkerId} not found");
            }

            var email = request.Email.Trim();

            var checkExistingEmail = await repo.Workers.GetByEmail(request.Email, cancellationToken);

            if(checkExistingEmail != null &&  checkExistingEmail.Id != request.WorkerId)
            {
                logger.LogWarning("Attempt to update client {ClientId} with duplicate email {Email}",
                    worker.Id,
                    email
                );

                throw new InvalidOperationException($"Email {email} is already in use.");

            }

            worker.UpdateName(request.LastName, request.FirstName, request.ModifiedBy);
            worker.UpdateContact(request.Contact, request.ModifiedBy);
            worker.UpdateEmail(request.Email, request.ModifiedBy);
            worker.UpdateAddress(request.Address, request.ModifiedBy);
            worker.UpdateDateOfBirth(request.DateOfBirth, request.ModifiedBy);
            worker.UpdateState(request.State, request.ModifiedBy);
            worker.UpdateGender(request.Gender, request.ModifiedBy);

            await repo.Workers.UpdateWorker(worker, cancellationToken);

            await repo.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}

