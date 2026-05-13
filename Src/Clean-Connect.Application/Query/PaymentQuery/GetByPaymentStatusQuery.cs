using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Query.PaymentQuery
{
    public record GetByPaymentStatusQuery(PaymentStatus status) : IRequest<List<PaymentDto>>;

    public class GetByPaymentStatusQueryHandler : IRequestHandler<GetByPaymentStatusQuery, List<PaymentDto>>
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<GetByPaymentStatusQueryHandler> logger;
        public GetByPaymentStatusQueryHandler(IUnitOfWork _repo, ILogger<GetByPaymentStatusQueryHandler> _logger)
        {
            repo = _repo;
            logger = _logger;
        }
        public async Task<List<PaymentDto>> Handle(GetByPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            var payments = await repo.Payments.GetByStatusAsync(request.status, cancellationToken);
            return payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaymentReference = payment.PaymentReference,
                CreatedAt = payment.DateCreated
            }).ToList();
        }
    }   


}
