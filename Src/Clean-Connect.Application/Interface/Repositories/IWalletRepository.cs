using Clean_Connect.Domain.Entities;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface IWalletRepository
    {
        Task CreateWallet(Wallet wallet, CancellationToken cancellationToken);
        Task<Wallet?> GetByWorkerId(Guid workerId, CancellationToken cancellationToken);
        Task UpdateWallet(Wallet wallet, CancellationToken cancellationToken);
    }
}
