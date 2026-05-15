using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Clean_Connect.Persistence.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CouponRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Coupon> CreateCouponAsync(Coupon coupon, CancellationToken cancellationToken)
        {
            await _dbContext.Coupons.AddAsync(coupon, cancellationToken);
            return coupon;
        }

        public async Task DeleteCouponAsync(Coupon coupon, CancellationToken cancellationToken)
        {
            _dbContext.Coupons.Remove(coupon);
            await Task.CompletedTask;
        }

        public async Task<List<Coupon>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Coupons.ToListAsync(cancellationToken);
        }

        public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        {
            return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant(), cancellationToken);
        }

        public async Task<Coupon?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
