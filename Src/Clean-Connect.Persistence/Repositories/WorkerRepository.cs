using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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
            return await dbContext.Workers.Include(s => s.ServiceType).FirstOrDefaultAsync(x => x.Id == workerId);
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
