using Clean_Connect.Domain.Enums;
using Clean_Connect.Domain.Utilities;

namespace Clean_Connect.Domain.Entities
{
    public class Escrow : BaseEntity
    {
        private Escrow() { }

        private Escrow(Guid bookingId, Guid paymentId, Guid workerId, decimal amount, string? createdBy = null)
        {
            ValidateId(bookingId, nameof(bookingId));
            ValidateId(paymentId, nameof(paymentId));
            ValidateId(workerId, nameof(workerId));
            ValidateAmount(amount);

            BookingId = bookingId;
            PaymentId = paymentId;
            WorkerId = workerId;
            Amount = amount;
            Status = EscrowStatus.Held;
            UpdateMetadata(createdBy);
        }

        public Guid BookingId { get; private set; }
        public Booking Booking { get; private set; } = default!;
        public Guid PaymentId { get; private set; }
        public Payment Payment { get; private set; } = default!;
        public Guid WorkerId { get; private set; }
        public Worker Worker { get; private set; } = default!;
        public decimal Amount { get; private set; }
        public EscrowStatus Status { get; private set; }
        public DateTime? DateReleased { get; private set; }

        public static Escrow Create(Guid bookingId, Guid paymentId, Guid workerId, decimal amount, string? createdBy = null)
        {
            return new Escrow(bookingId, paymentId, workerId, amount, createdBy);
        }

        public void Release(string? modifiedBy = null)
        {
            if (Status == EscrowStatus.Released)
                throw new InvalidOperationException("Escrow has already been released.");

            if (Status != EscrowStatus.Held)
                throw new InvalidOperationException($"Escrow cannot be released from {Status} status.");

            Status = EscrowStatus.Released;
            DateReleased = DateTime.UtcNow;
            UpdateMetadata(modifiedBy);
        }

        private static void ValidateId(Guid id, string paramName)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        }

        private static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        }
    }
}
