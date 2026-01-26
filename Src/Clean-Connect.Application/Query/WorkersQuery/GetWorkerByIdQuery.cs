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
    public record GetWorkerByIdQuery(Guid Id) : IRequest<WorkerDto>;

    public class GetWorkerByIdQueryHandler : IRequestHandler<GetWorkerByIdQuery, WorkerDto>
    {

        private readonly IUnitOfWork repo;
        private readonly ILogger<GetWorkerByIdQueryHandler> logger;

        public GetWorkerByIdQueryHandler(IUnitOfWork _repo, ILogger<GetWorkerByIdQueryHandler> _logger)
        {
            repo = _repo;
            logger = _logger;
        }
        public async Task<WorkerDto> Handle(GetWorkerByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                logger.LogError("GetWorkerByIdQueryHandler: Invalid Id provided.");
                throw new KeyNotFoundException("Invalid Id provided.");
            }

            var check = await repo.Workers.GetWorkerById(request.Id, cancellationToken);

            if (check == null)
            {
                logger.LogWarning("GetWorkerByIdQueryHandler: Worker with Id {WorkerId} not found.", request.Id);
                throw new ArgumentException($"Worker with Id {request.Id} not found.");
            }

            var worker = new WorkerDto
            {
                Name = check.FullName,
                Address = check.Address,
                Contact = check.Contact,
                DateOfBirth = check.DateOfBirth,
                ServiceType = check.ServiceType.Name,
                Age = check.Age,
                State = check.State,
                Email = check.Email,
                Gender = check.Gender.ToString()
            };

            logger.LogInformation("GetWorkerByIdQueryHandler: Successfully retrieved worker with Id {WorkerId}.", request.Id);

            return worker;
        }
    }
}