using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Clean_Connect.Persistence.Repositories
{
    public class WalletRepository(ApplicationDbContext dbContext) : IWalletRepository
    {
        public async Task CreateWallet(Wallet wallet, CancellationToken cancellationToken)
        {
            await dbContext.Wallets.AddAsync(wallet, cancellationToken);
        }

        public async Task<Wallet?> GetByWorkerId(Guid workerId, CancellationToken cancellationToken)
        {
            return await dbContext.Wallets
                .FirstOrDefaultAsync(x => x.WorkerId == workerId, cancellationToken);
        }

        public Task UpdateWallet(Wallet wallet, CancellationToken cancellationToken)
        {
            dbContext.Wallets.Update(wallet);
            return Task.CompletedTask;
        }
    }
}
