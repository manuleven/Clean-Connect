using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Query.WorkersQuery
{
    public record GetAllWorkersQuery : IRequest<List<WorkerDto>>;

    public class GetAllWorkersQueryHandler : IRequestHandler<GetAllWorkersQuery, List<WorkerDto>>
    {

        private readonly IUnitOfWork _repo;
        private readonly ILogger<GetAllWorkersQuery> _logger;

        public GetAllWorkersQueryHandler(IUnitOfWork repo, ILogger<GetAllWorkersQuery> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        public async Task<List<WorkerDto>> Handle(GetAllWorkersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllWorkersQuery");

            var workers = await _repo.Workers.GetAllWorkers(cancellationToken);

            if (workers == null || !workers.Any())
            {
                _logger.LogWarning("No workers found");
                return new List<WorkerDto>();
            }


            var workersList = workers.Select(worker => new WorkerDto
            {
                Name = worker.FullName,
                Email = worker.Email,
                Address = worker.Address,
                Contact = worker.Contact,
                Gender = worker.Gender.ToString(),
                DateOfBirth = worker.DateOfBirth,

            }).ToList();

            _logger.LogInformation("Successfully retrieved {Count} workers", workersList.Count);

            return workersList;
        }

    }
}
