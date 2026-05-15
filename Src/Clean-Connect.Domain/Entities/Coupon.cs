using Clean_Connect.Domain.Utilities;
using System;

namespace Clean_Connect.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        private Coupon() { }

        private Coupon(string code, decimal discountPercentage, DateTime expirationDate, int usageLimit, string? createdBy = null)
        {
            Code = code.ToUpperInvariant();
            DiscountPercentage = discountPercentage;
            ExpirationDate = expirationDate;
            UsageLimit = usageLimit;
            UsedCount = 0;
            IsActive = true;
            UpdateMetadata(createdBy);
        }

        public string Code { get; private set; } = default!;
        public decimal DiscountPercentage { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public int UsageLimit { get; private set; }
        public int UsedCount { get; private set; }
        public bool IsActive { get; private set; }

        public static Coupon Create(string code, decimal discountPercentage, DateTime expirationDate, int usageLimit, string? createdBy = null)
        {
            ValidateCode(code);
            ValidateDiscount(discountPercentage);
            ValidateExpiration(expirationDate);

            return new Coupon(code, discountPercentage, expirationDate, usageLimit, createdBy);
        }

        public void Update(string code, decimal discountPercentage, DateTime expirationDate, int usageLimit, bool isActive, string? modifiedBy = null)
        {
            ValidateCode(code);
            ValidateDiscount(discountPercentage);

            Code = code.ToUpperInvariant();
            DiscountPercentage = discountPercentage;
            ExpirationDate = expirationDate;
            UsageLimit = usageLimit;
            IsActive = isActive;
            UpdateMetadata(modifiedBy);
        }

        public void Deactivate(string? modifiedBy = null)
        {
            IsActive = false;
            UpdateMetadata(modifiedBy);
        }

        public void IncrementUsage(string? modifiedBy = null)
        {
            if (!IsValid())
            {
                throw new InvalidOperationException($"Coupon {Code} is not valid for usage.");
            }

            UsedCount++;
            UpdateMetadata(modifiedBy);
        }

        public decimal ApplyDiscount(decimal originalAmount)
        {
            if (!IsValid())
            {
                throw new InvalidOperationException($"Coupon {Code} is not valid.");
            }

            var discount = originalAmount * (DiscountPercentage / 100m);
            return originalAmount - discount;
        }

        public bool IsValid()
        {
            return IsActive && ExpirationDate > DateTime.UtcNow && UsedCount < UsageLimit;
        }

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code cannot be empty.", nameof(code));
        }

        private static void ValidateDiscount(decimal discountPercentage)
        {
            if (discountPercentage <= 0 || discountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 1 and 100.", nameof(discountPercentage));
        }

        private static void ValidateExpiration(DateTime expirationDate)
        {
            if (expirationDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiration date must be in the future.", nameof(expirationDate));
        }
    }
}
