using Clean_Connect.Application.Interface.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.CouponCommand
{
   public record UpdateCouponCommand(Guid Id, string Code, decimal DiscountPercentage, DateTime ExpirationDate, int UsageLimit, bool IsActive, string? ModifiedBy = null) : IRequest<bool>;

    public class UpdateCouponValidator : AbstractValidator<UpdateCouponCommand>
    {
        public UpdateCouponValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Coupon ID is required.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("Coupon code is required.");
            RuleFor(x => x.DiscountPercentage).InclusiveBetween(1, 100).WithMessage("Discount percentage must be between 1 and 100.");
        }
    }

    public class UpdateCouponHandler : IRequestHandler<UpdateCouponCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCouponHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken);
            if (coupon == null)
                throw new KeyNotFoundException($"Coupon with ID {request.Id} not found.");

            // Check if the new code exists and is not the current coupon
            var existingCoupon = await _unitOfWork.Coupons.GetByCodeAsync(request.Code, cancellationToken);
            if (existingCoupon != null && existingCoupon.Id != request.Id)
                throw new ValidationException("Another coupon with this code already exists.");

            coupon.Update(request.Code, request.DiscountPercentage, request.ExpirationDate, request.UsageLimit, request.IsActive, request.ModifiedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}   