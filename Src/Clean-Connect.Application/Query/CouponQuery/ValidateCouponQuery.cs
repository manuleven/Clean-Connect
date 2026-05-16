using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Query.CouponQuery
{
    public record ValidateCouponQuery(string Code) : IRequest<CouponDto>;

    public class ValidateCouponHandler : IRequestHandler<ValidateCouponQuery, CouponDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidateCouponHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CouponDto> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.Coupons.GetByCodeAsync(request.Code, cancellationToken);
            if (coupon == null)
                throw new ValidationException("Coupon does not exist.");

            if (!coupon.IsValid())
                throw new ValidationException("Coupon is expired or usage limit reached.");

            return new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountPercentage = coupon.DiscountPercentage,
                ExpirationDate = coupon.ExpirationDate,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                IsActive = coupon.IsActive,
                IsValid = true
            };
        }
    }
}
