using Clean_Connect.Domain.Enums;

namespace Clean_Connect.Application.DTO
{
    public record WalletDto(Guid WorkerId, decimal Balance, decimal TotalEarned);

    public record EscrowDto(
        Guid BookingId,
        Guid PaymentId,
        Guid WorkerId,
        decimal Amount,
        EscrowStatus Status,
        DateTime? DateReleased);
}
