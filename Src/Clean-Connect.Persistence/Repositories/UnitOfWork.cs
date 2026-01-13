using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Infrastructure.Context;

namespace Clean_Connect.Persistence.Repositories
{
    public class UnitOfWork(ApplicationDbContext dbContext, IWorkerRepository workerRepository, IClientRepository clientRepository) : IUnitOfWork
    {
        public IWorkerRepository Workers { get;  } = workerRepository;

        public IClientRepository Clients { get; } = clientRepository;

        public async Task <int> SaveChangesAsync(CancellationToken cancellation)
        {
            return await dbContext.SaveChangesAsync(cancellation);
        }
    }
}
