using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.Services
{
    public class EscrowService
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<EscrowService> logger;

        public EscrowService(IUnitOfWork repo, ILogger<EscrowService> logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        public async Task HoldPaymentInEscrowAsync(Booking booking, Payment payment, CancellationToken cancellationToken)
        {
            if (payment.Status != PaymentStatus.Successful)
                throw new InvalidOperationException("Only successful payments can be held in escrow.");

            if (booking.PaymentStatus != PaymentStatus.Successful)
                throw new InvalidOperationException("Booking payment must be successful before escrow can be created.");

            var existingEscrow = await repo.Escrows.GetByBookingId(booking.Id, cancellationToken);
            if (existingEscrow != null)
            {
                logger.LogInformation("Escrow already exists for booking {BookingId}", booking.Id);
                return;
            }

            var escrow = Escrow.Create(
                booking.Id,
                payment.Id,
                booking.WorkerId,
                payment.Amount);

            await repo.Escrows.CreateEscrow(escrow, cancellationToken);

            logger.LogInformation(
                "Held {Amount} in escrow for booking {BookingId} and worker {WorkerId}",
                payment.Amount,
                booking.Id,
                booking.WorkerId);
        }

        public async Task ReleaseEscrowToWorkerWalletAsync(Booking booking, string? modifiedBy, CancellationToken cancellationToken)
        {
            if (booking.BookingStatus != BookingStatus.Completed)
                throw new InvalidOperationException("Escrow can only be released after the booking is completed.");

            if (booking.PaymentStatus != PaymentStatus.Successful)
                throw new InvalidOperationException("Escrow can only be released for a paid booking.");

            var escrow = await repo.Escrows.GetByBookingId(booking.Id, cancellationToken)
                ?? throw new InvalidOperationException($"Escrow for booking {booking.Id} was not found.");

            if (escrow.Status == EscrowStatus.Released)
            {
                logger.LogInformation("Escrow for booking {BookingId} was already released", booking.Id);
                return;
            }

            if (escrow.Status != EscrowStatus.Held)
                throw new InvalidOperationException($"Escrow cannot be released from {escrow.Status} status.");

            var wallet = await repo.Wallets.GetByWorkerId(booking.WorkerId, cancellationToken);
            if (wallet == null)
            {
                wallet = Wallet.Create(booking.WorkerId, modifiedBy);
                await repo.Wallets.CreateWallet(wallet, cancellationToken);
            }

            wallet.Credit(escrow.Amount, modifiedBy);
            escrow.Release(modifiedBy);

            await repo.Wallets.UpdateWallet(wallet, cancellationToken);
            await repo.Escrows.UpdateEscrow(escrow, cancellationToken);

            logger.LogInformation(
                "Released {Amount} from escrow for booking {BookingId} to worker {WorkerId}",
                escrow.Amount,
                booking.Id,
                booking.WorkerId);
        }
    }
}
