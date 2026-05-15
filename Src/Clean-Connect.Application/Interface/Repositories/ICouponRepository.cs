using Clean_Connect.Domain.Entities;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface ICouponRepository
    {
        Task<Coupon> CreateCouponAsync(Coupon coupon, CancellationToken cancellationToken);
        Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken);
        Task<List<Coupon>> GetAllAsync(CancellationToken cancellationToken);
        Task DeleteCouponAsync(Coupon coupon, CancellationToken cancellationToken);
    }
}
