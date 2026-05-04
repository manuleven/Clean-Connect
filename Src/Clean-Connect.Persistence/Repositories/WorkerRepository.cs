using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Helper;
using Clean_Connect.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Clean_Connect.Persistence.Repositories
{
    public class WorkerRepository(ApplicationDbContext dbContext) : IWorkerRepository
    {
        public async Task CreateWorker(Worker worker, CancellationToken cancellationToken)
        {
            await dbContext.Workers.AddAsync(worker);
        }

        public async Task <Worker> GetWorkerByName(string name, CancellationToken cancellationToken)
        {
          return await dbContext.Workers.FindAsync(name.Trim(), cancellationToken);
        }

        public async Task <Worker> GetByEmail(string email, CancellationToken cancellationToken)
        {
            return await dbContext.Workers.FirstOrDefaultAsync(x  => x.Email.Value == email.Trim());
            
        }
        public async Task<Worker> GetWorkerById(Guid workerId, CancellationToken cancellationToken)
        {
            return await dbContext.Workers
                .Include(s => s.ServiceType)
                .Include(s => s.Bookings)
                .ThenInclude(c => c.Client)
                .Include(s => s.Bookings)
                .ThenInclude(c => c.Ratings)
                .Include(s => s.Bookings)
                .ThenInclude(c => c.ServiceType)        
                .FirstOrDefaultAsync(x => x.Id == workerId);
        }

        public async Task<List<WorkerWithDistance>> GetNearByWorkersAsync(double latitude,double longitude,double radiusInMeters,Guid serviceType)
        {
            var location = new Point(longitude, latitude)
            {
                SRID = 4326
            };

            var result = await dbContext.Workers
        .Include(s => s.ServiceType)
        .Where(w => w.Location != null &&
                    w.ServiceTypeId == serviceType &&
                    w.Location.Point.IsWithinDistance(location, radiusInMeters))
        .Select(w => new
        {
            Worker = w,
            Distance = w.Location.Point.Distance(location) / 1000.0
        })
        .OrderBy(x => x.Distance)
        .ThenByDescending(x => x.Worker.AverageRating)
        .ToListAsync(); // ✅ still EF here

            // switch to memory AFTER query executes
            return result
                .Select(x => new WorkerWithDistance
                {
                    Worker = x.Worker,
                    DistanceInKm = x.Distance
                })
                .ToList(); // ✅ now normal LINQ
        }
        public async Task<IEnumerable<Worker>> GetAllWorkers(CancellationToken cancellationToken)
        {
            return await dbContext.Workers.Include(s => s.ServiceType).ToListAsync(cancellationToken);
        }

        public async Task UpdateWorker(Worker worker, CancellationToken cancellationToken)
        {
            dbContext.Workers.Update(worker);
        }

        public async Task DeleteWorker(Worker worker, CancellationToken cancellationToken)
        {
            dbContext.Workers.Remove(worker);
        }
    }
}
