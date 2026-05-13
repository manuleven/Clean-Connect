using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Microsoft.Extensions.Logging;


namespace Clean_Connect.Application.Command.Services
{
    public class PayoutService
    {
        private readonly IUnitOfWork _repo;
        private readonly ILogger<PayoutService> _logger;

        public PayoutService(IUnitOfWork repo, ILogger<PayoutService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task PayoutAsync(Booking booking, string? modifiedBy, CancellationToken cancellationToken)
        {
            if (booking == null) throw new ArgumentNullException(nameof(booking));

            if (booking.PaymentStatus != PaymentStatus.Successful)
            {
                _logger.LogWarning("Payout aborted. Booking {BookingId} payment status is {PaymentStatus}", booking.Id, booking.PaymentStatus);
                throw new InvalidOperationException("Payout can only be performed for paid bookings.");
            }

            var escrow = await _repo.Escrows.GetByBookingId(booking.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Escrow for booking {booking.Id} not found.");

            if (escrow.Status != EscrowStatus.Held)
            {
                _logger.LogInformation("Escrow for booking {BookingId} is not in held state ({Status}). No payout performed.", booking.Id, escrow.Status);
                return;
            }

            var wallet = await _repo.Wallets.GetByWorkerId(booking.WorkerId, cancellationToken);
            if (wallet == null)
            {
                wallet = Wallet.Create(booking.WorkerId, modifiedBy);
                await _repo.Wallets.CreateWallet(wallet, cancellationToken);
            }

            wallet.Credit(escrow.Amount, modifiedBy);
            escrow.Release(modifiedBy);

            await _repo.Wallets.UpdateWallet(wallet, cancellationToken);
            await _repo.Escrows.UpdateEscrow(escrow, cancellationToken);

            await _repo.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payout completed: {Amount} transferred from escrow for booking {BookingId} to worker {WorkerId}", escrow.Amount, booking.Id, booking.WorkerId);
        }
    }
}