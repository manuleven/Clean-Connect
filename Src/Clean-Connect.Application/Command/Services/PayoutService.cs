using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Application.Interface.Services;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Command.Services
{
    /// <summary>
    /// Orchestrates payout workflow: determines internal wallet credit vs external transfer,
    /// delegates to PaystackService for provider calls, manages escrow release, and persists changes.
    /// </summary>
    public class PayoutService
    {
        private readonly IUnitOfWork _repo;
        private readonly IPaystackService _paystackService;
        private readonly WalletService _walletService;
        private readonly EscrowService _escrowService;
        private readonly ILogger<PayoutService> _logger;

        public PayoutService(
            IUnitOfWork repo,
            IPaystackService paystackService,
            WalletService walletService,
            EscrowService escrowService,
            ILogger<PayoutService> logger)
        {
            _repo = repo;
            _paystackService = paystackService;
            _walletService = walletService;
            _escrowService = escrowService;
            _logger = logger;
        }

        public async Task<PayoutResult> PayoutAsync(Booking booking, WorkerBankAccountDto? bankAccount, string? modifiedBy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting payout orchestration for booking: {BookingId}, worker: {WorkerId}", booking.Id, booking.WorkerId);

            if (booking.PaymentStatus != PaymentStatus.Successful)
            {
                _logger.LogWarning("Payout aborted for booking {BookingId}. Payment status is {PaymentStatus}, not Successful.", booking.Id, booking.PaymentStatus);
                return new PayoutResult(false, "Booking must be paid before payout.");
            }

            var escrow = await _repo.Escrows.GetByBookingId(booking.Id, cancellationToken);
            if (escrow == null)
            {
                _logger.LogError("Escrow not found for booking: {BookingId}", booking.Id);
                throw new InvalidOperationException($"Escrow not found for booking {booking.Id}");
            }

            if (escrow.Status != EscrowStatus.Held)
            {
                _logger.LogWarning("Payout aborted for booking {BookingId}. Escrow status is {EscrowStatus}, not Held.", booking.Id, escrow.Status);
                return new PayoutResult(false, $"Escrow is not held (current status={escrow.Status}).");
            }

            // Internal wallet flow
            if (bankAccount == null)
            {
                _logger.LogInformation("No bank account provided for booking {BookingId}. Using internal wallet payout.", booking.Id);
                return await PayoutToInternalWalletAsync(booking, escrow, modifiedBy, cancellationToken);
            }

            // External transfer flow
            _logger.LogInformation("Bank account provided for booking {BookingId}. Initiating external transfer payout.", booking.Id);
            return await PayoutToExternalTransferAsync(booking, bankAccount, escrow, modifiedBy, cancellationToken);
        }

        private async Task<PayoutResult> PayoutToInternalWalletAsync(Booking booking, Escrow escrow, string? modifiedBy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing internal wallet payout for booking: {BookingId}", booking.Id);

            try
            {
                // Delegate wallet/escrow operations to EscrowService
                await _escrowService.ReleaseEscrowToWorkerWalletAsync(booking, modifiedBy, cancellationToken);

                // Persist repository changes
                await _repo.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Internal wallet payout completed successfully for booking: {BookingId}. Amount: {Amount}", booking.Id, escrow.Amount);
                return new PayoutResult(true, "Credited worker internal wallet.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal wallet payout failed for booking: {BookingId}", booking.Id);
                throw;
            }
        }

        private async Task<PayoutResult> PayoutToExternalTransferAsync(Booking booking, WorkerBankAccountDto bankAccount, Escrow escrow, string? modifiedBy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing external transfer payout for booking: {BookingId}, account: {AccountName}", booking.Id, bankAccount.AccountName);

            try
            {
                // 1) Create transfer recipient via PaystackService
                _logger.LogDebug("Creating transfer recipient for booking: {BookingId}", booking.Id);
                var recipient = await _paystackService.CreateTransferRecipientAsync(bankAccount, cancellationToken);
                _logger.LogInformation("Transfer recipient created. RecipientCode: {RecipientCode}", recipient.RecipientCode);

                // 2) Initiate transfer via PaystackService
                var reason = $"Payout for booking {booking.Id}";
                _logger.LogDebug("Initiating transfer for booking: {BookingId}, amount: {Amount}", booking.Id, escrow.Amount);
                var transferResult = await _paystackService.InitiateTransferAsync(recipient.RecipientCode, escrow.Amount, reason, cancellationToken);
                _logger.LogInformation("Transfer initiated. TransferCode: {TransferCode}, Status: {Status}", transferResult.TransferCode, transferResult.Status);

                // 3) Release escrow only after provider accepted the transfer
                _logger.LogDebug("Releasing escrow for booking: {BookingId}", booking.Id);
                escrow.Release(modifiedBy);
                await _repo.Escrows.UpdateEscrow(escrow, cancellationToken);

                // 4) Persist state
                await _repo.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("External transfer payout completed successfully for booking: {BookingId}. ProviderRef: {ProviderRef}", 
                    booking.Id, transferResult.TransferCode);
                return new PayoutResult(true, "External transfer initiated.", transferResult.TransferCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External transfer payout failed for booking: {BookingId}", booking.Id);
                throw;
            }
        }
    }
}