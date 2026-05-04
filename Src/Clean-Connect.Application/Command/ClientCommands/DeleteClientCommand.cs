using Clean_Connect.Application.Interface.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.ClientCommands
{
    public record DeleteClientCommand(Guid id) : IRequest<bool>;
 

    public class DeleteClientValidator : AbstractValidator<DeleteClientCommand>
    {
        public DeleteClientValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty()
                .WithMessage("Client ID cannot be empty.")
                .Must(id => id != Guid.Empty)
                .WithMessage("Client ID cannot be an empty GUID.");
        }
    }

    public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, bool>
    {
        private readonly IUnitOfWork _repo;
        public DeleteClientCommandHandler(IUnitOfWork repo)
        {
            _repo = repo;
        }
        public async Task<bool> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            var client = await _repo.Clients.GetClientById(request.id, cancellationToken);
            if (client == null)
            {
                throw new KeyNotFoundException($"Client with ID {request.id} not found.");
            }
            _repo.Clients.DeleteClient(client, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
