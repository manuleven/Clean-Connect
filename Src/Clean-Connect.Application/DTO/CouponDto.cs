using System;

namespace Clean_Connect.Application.DTO
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
    }
}
