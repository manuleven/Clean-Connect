using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.CouponCommand
{
      public record CreateCouponCommand(string Code, decimal DiscountPercentage, DateTime ExpirationDate, int UsageLimit, string? CreatedBy = null) : IRequest<Guid>;

    public class CreateCouponValidator : AbstractValidator<CreateCouponCommand>
    {
        public CreateCouponValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Coupon code is required.");
            RuleFor(x => x.DiscountPercentage).InclusiveBetween(1, 100).WithMessage("Discount percentage must be between 1 and 100.");
            RuleFor(x => x.ExpirationDate).GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.");
            RuleFor(x => x.UsageLimit).GreaterThan(0).WithMessage("Usage limit must be greater than zero.");
        }
    }

    public class CreateCouponHandler : IRequestHandler<CreateCouponCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCouponHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            var existingCoupon = await _unitOfWork.Coupons.GetByCodeAsync(request.Code, cancellationToken);
            if (existingCoupon != null)
                throw new ValidationException("Coupon code already exists.");

            var coupon = Coupon.Create(request.Code, request.DiscountPercentage, request.ExpirationDate, request.UsageLimit, request.CreatedBy);
            await _unitOfWork.Coupons.CreateCouponAsync(coupon, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return coupon.Id;
        }
    }
}