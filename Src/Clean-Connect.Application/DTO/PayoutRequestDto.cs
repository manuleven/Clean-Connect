namespace Clean_Connect.Application.DTO
{
    /// <summary>
    /// Request body for triggering a payout on a completed booking.
    /// </summary>
    public record PayoutRequest(
        Guid WorkerId,
        string? AccountNumber = null,
        string? BankCode = null,
        string? AccountName = null,
        string Currency = "NGN",
        string? ModifiedBy = null);

    /// <summary>
    /// Request body for a worker-initiated payout from wallet.
    /// </summary>
    public record WorkerPayoutRequest(
        Guid BookingId,
        string? AccountNumber = null,
        string? BankCode = null,
        string? AccountName = null,
        string Currency = "NGN",
        string? ModifiedBy = null);
}
