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
    public record GetAllClientQuery : IRequest<List<ClientDto>>;

    public class GetAllClientQueryHandler : IRequestHandler<GetAllClientQuery, List<ClientDto>>
    {

        private readonly IUnitOfWork repo;
        private readonly ILogger<GetAllClientQueryHandler> logger;

        public GetAllClientQueryHandler(IUnitOfWork _repo, ILogger<GetAllClientQueryHandler> _logger)
        {
            repo = _repo;
            logger = _logger;
        }
        public async Task<List<ClientDto>> Handle(GetAllClientQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling GetAllClientQuery");

            var clients = await repo.Clients.GetAllClients(cancellationToken);

            if (clients == null || !clients.Any())
            {
                logger.LogWarning("No clients found");
                return new List<ClientDto>();
            }

            var clientDtos = clients.Select(client => new ClientDto
            {
                Name = client.FullName,
                Email = client.Email,
                Address = client.Address,
                Age = client.Age,
                Contact = client.PhoneNumber,
                DateOfBirth = client.DateOfBirth,
                Gender = client.Gender.ToString(),
                State = client.State
            }).ToList();

            logger.LogInformation("Successfully retrieved {Count} clients", clientDtos.Count);

            return clientDtos;
        }
    }
}
