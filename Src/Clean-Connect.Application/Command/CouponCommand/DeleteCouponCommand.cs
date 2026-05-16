using Clean_Connect.Application.Interface.Repositories;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.CouponCommand
{
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