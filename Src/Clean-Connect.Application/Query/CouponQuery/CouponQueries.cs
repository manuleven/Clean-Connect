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
    // Get All Coupons
    public record GetAllCouponsQuery() : IRequest<List<CouponDto>>;

    public class GetAllCouponsHandler : IRequestHandler<GetAllCouponsQuery, List<CouponDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCouponsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CouponDto>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
        {
            var coupons = await _unitOfWork.Coupons.GetAllAsync(cancellationToken);
            return coupons.Select(c => new CouponDto
            {
                Id = c.Id,
                Code = c.Code,
                DiscountPercentage = c.DiscountPercentage,
                ExpirationDate = c.ExpirationDate,
                UsageLimit = c.UsageLimit,
                UsedCount = c.UsedCount,
                IsActive = c.IsActive,
                IsValid = c.IsValid()
            }).ToList();
        }
    }

    // Get Coupon By Code
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

    // Validate Coupon
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
