using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Infrastructure.Context;

namespace Clean_Connect.Persistence.Repositories
{
    public class UnitOfWork(ApplicationDbContext dbContext, IWorkerRepository workerRepository, IClientRepository clientRepository, IServiceTypeRepository serviceTypeRepository) : IUnitOfWork
    {
        public IWorkerRepository Workers { get;  } = workerRepository;


        public IClientRepository Clients { get; } = clientRepository;

        public async Task BeginTransactionAsync(CancellationToken cancellation)
        {
            await dbContext.Database.BeginTransactionAsync(cancellation);
        }   

        public async Task CommitTransactionAsync(CancellationToken cancellation)
        {
            await dbContext.Database.CommitTransactionAsync(cancellation);
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellation)
        {
            await dbContext.Database.RollbackTransactionAsync(cancellation);
        }
        public IServiceTypeRepository ServiceTypes { get; } = serviceTypeRepository;

        public async Task <int> SaveChangesAsync(CancellationToken cancellation)
        {
            return await dbContext.SaveChangesAsync(cancellation);
        }
    }
}
