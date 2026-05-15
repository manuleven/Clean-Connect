using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.CouponCommand
{
    // Create Coupon
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

    // Update Coupon
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

    // Delete Coupon
    public record DeleteCouponCommand(Guid Id) : IRequest<bool>;

    public class DeleteCouponHandler : IRequestHandler<DeleteCouponCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCouponHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken);
            if (coupon == null)
                throw new KeyNotFoundException($"Coupon with ID {request.Id} not found.");

            await _unitOfWork.Coupons.DeleteCouponAsync(coupon, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
