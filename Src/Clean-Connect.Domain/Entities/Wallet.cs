using Clean_Connect.Domain.Utilities;

namespace Clean_Connect.Domain.Entities
{
    public class Wallet : BaseEntity
    {
        private Wallet() { }

        private Wallet(Guid workerId, string? createdBy = null)
        {
            ValidateWorkerId(workerId);

            WorkerId = workerId;
            Balance = 0;
            TotalEarned = 0;
            UpdateMetadata(createdBy);
        }

        public Guid WorkerId { get; private set; }
        public Worker Worker { get; private set; } = default!;
        public decimal Balance { get; private set; }
        public decimal TotalEarned { get; private set; }

        public static Wallet Create(Guid workerId, string? createdBy = null)
        {
            return new Wallet(workerId, createdBy);
        }

        public void Credit(decimal amount, string? modifiedBy = null)
        {
            ValidateAmount(amount);

            Balance += amount;
            TotalEarned += amount;
            UpdateMetadata(modifiedBy);
        }

        private static void ValidateWorkerId(Guid workerId)
        {
            if (workerId == Guid.Empty)
                throw new ArgumentException("WorkerId cannot be empty.", nameof(workerId));
        }

        private static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        }
    }
}
