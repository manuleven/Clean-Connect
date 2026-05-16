using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Query.CouponQuery
{
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
}
