using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Query.ClientQuery
{
    public record GetClientByIdQuery(Guid Id) : IRequest<ClientDto>;

    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDto>
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<GetClientByIdQueryHandler> logger;

        public GetClientByIdQueryHandler(IUnitOfWork _repo, ILogger<GetClientByIdQueryHandler> _logger)
        {
            repo = _repo;
            logger = _logger;
        }

        public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetClientByIdHandler");
            if (request.Id == Guid.Empty)
            {
                logger.LogError("GetClientByIdQueryHandler: Invalid Id provided.");
                throw new KeyNotFoundException("Invalid Id provided.");
            }

            var check = await repo.Clients.GetClientById(request.Id, cancellationToken);

            if (check == null)
            {
                logger.LogWarning("GetClientByIdQueryHandler: Client with Id {ClientId} not found.", request.Id);
                throw new ArgumentException($"Client with Id {request.Id} not found.");
            }

            var client = new ClientDto
            {
                Name = check.FullName,
                Address = check.Address,
                State = check.State,
                DateOfBirth = check.DateOfBirth,
                Gender = check.Gender.ToString(),
                Age = check.Age,
                Email = check.Email,
                Contact = check.PhoneNumber
            };

            logger.LogInformation("GetWorkerByIdQueryHandler: Successfully retrieved worker with Id {WorkerId}.", request.Id);

            return client;
        }
    }

}
