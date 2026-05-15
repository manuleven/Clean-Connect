using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.Services
{
    public class WalletService
    {
        private readonly IUnitOfWork _repo;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IUnitOfWork repo, ILogger<WalletService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<Wallet> GetOrCreateWalletAsync(Guid workerId, string? createdBy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving wallet for worker: {WorkerId}", workerId);

            var wallet = await _repo.Wallets.GetByWorkerId(workerId, cancellationToken);
            if (wallet == null)
            {
                _logger.LogInformation("Wallet not found for worker: {WorkerId}. Creating new wallet.", workerId);
                wallet = Wallet.Create(workerId, createdBy);
                await _repo.Wallets.CreateWallet(wallet, cancellationToken);
                _logger.LogInformation("Wallet created successfully for worker: {WorkerId}", workerId);
            }
            else
            {
                _logger.LogInformation("Wallet found for worker: {WorkerId}. Balance: {Balance}, TotalEarned: {TotalEarned}",
                    workerId, wallet.Balance, wallet.TotalEarned);
            }

            return wallet;
        }

        public async Task CreditWalletAsync(Wallet wallet, decimal amount, string? modifiedBy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Crediting wallet for worker: {WorkerId} with amount: {Amount}", wallet.WorkerId, amount);

            wallet.Credit(amount, modifiedBy);
            await _repo.Wallets.UpdateWallet(wallet, cancellationToken);

            _logger.LogInformation("Wallet credited successfully. New balance: {NewBalance}, TotalEarned: {TotalEarned}",
                wallet.Balance, wallet.TotalEarned);
        }
    }
}
