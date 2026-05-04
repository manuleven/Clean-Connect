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
    public record GetNearByWorkersQuery(double Latitude, double Longitude, double RadiusInMeters, Guid ServiceType ) : IRequest<List<NearbyWorkerDto>>;


    public class GetNearByWorkersHandler: IRequestHandler<GetNearByWorkersQuery, List<NearbyWorkerDto>>
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<GetNearByWorkersHandler> logger;

        public GetNearByWorkersHandler(IUnitOfWork _repo, ILogger<GetNearByWorkersHandler> _logger)
        {
            repo = _repo;
            logger = _logger;
        }
        public async Task<List<NearbyWorkerDto>> Handle(GetNearByWorkersQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Nearby worker operation");
            var workers = await repo.Workers.GetNearByWorkersAsync(request.Latitude, request.Longitude, request.RadiusInMeters, request.ServiceType);
            logger.LogInformation(" Fetching Nearby worker operation");

            if (workers == null)
            {
                logger.LogWarning("Workers is NULL");
                throw new Exception("Workers is NULL");
            }

            if (!workers.Any())
            {
                logger.LogWarning("Workers is EMPTY");
                throw new Exception("Workers is EMPTY");
            }

            var workerDtos = workers.Select(x => new NearbyWorkerDto
            {
                Name = x.Worker.FullName,
                Email = x.Worker.Email,
                Rating = x.Worker.AverageRating,
                Address = x.Worker.Address,
                Contact = x.Worker.Contact,
                Gender = x.Worker.Gender.ToString(),
                DistanceInKm = Math.Round(x.DistanceInKm, 2),
                ServiceType = x.Worker.ServiceType.Name,
            }).ToList();
           

            return workerDtos;
        }
    }
}