using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Query.CouponQuery
{
    public record GetCouponByCodeQuery(string Code) : IRequest<CouponDto?>;

    public class GetCouponByCodeHandler : IRequestHandler<GetCouponByCodeQuery, CouponDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCouponByCodeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CouponDto?> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(request.Code, cancellationToken);
            if (coupon == null)
                return null;

            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountPercentage = coupon.DiscountPercentage,
                ExpirationDate = coupon.ExpirationDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                IsActive = coupon.IsActive,
                IsValid = coupon.IsValid()
            };
        }
    }

}
