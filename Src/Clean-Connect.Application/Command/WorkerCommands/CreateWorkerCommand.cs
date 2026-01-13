using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Clean_Connect.Domain.Value_Objects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata.Ecma335;

namespace Clean_Connect.Application.Command.WorkerCommands
{
    public record CreateWorkerCommand(string FirstName, string LastName, string Email, string Contact, string Address, Gender Gender, string State, DateTime Dob, string? CreatedBy = null) : IRequest<bool>;

    public class RegisterWorkerValidator : AbstractValidator<CreateWorkerCommand>
    {
        public RegisterWorkerValidator()
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
                .IsInEnum()
                .WithMessage("Gender must be Male or Female");

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

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address is required")
                .Length(10, 200)
                .WithMessage("Address must be between 10-200 characters");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required")
                .Length(3, 15).WithMessage("State name must be 3-15 characters");

            RuleFor(x => x.Dob)
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth cannot be in the future");

        }
    }
    public class CreateWorkerHandler(IUnitOfWork repo, ILogger<CreateWorkerHandler> logger) : IRequestHandler<CreateWorkerCommand, bool>
    {
        public async Task<bool> Handle(CreateWorkerCommand request, CancellationToken cancellationToken)
        {
            var checkExistingEmail = await repo.Workers.GetByEmail(request.Email, cancellationToken);

            if (checkExistingEmail != null)
            {
                logger.LogWarning("Worker creation failed. Email already in use: {Email}", request.Email);
                throw new ValidationException("Email already in use");
            }




            var fullname = FullName.Create(request.FirstName, request.LastName);
            var email = Email.Create(request.Email);
            var address = Address.Create(request.Address);
            var contact = PhoneNumber.Create(request.Contact);



            var worker = Worker.Create(
                fullname,
                address,
                contact,
                request.Gender,
                email,
                request.State,
                request.Dob,
                request.CreatedBy
                );

            await repo.Workers.CreateWorker(worker, cancellationToken);
            await repo.SaveChangesAsync(cancellationToken);

            logger.LogInformation("CreateWorker succeeded. WorkerId={WorkerId}, Email={Email}", worker.Id, request.Email);
            return true;
        }
    }

}
