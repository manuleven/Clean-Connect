using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Salaros.Configuration.Logging;

namespace Clean_Connect.Application.Query.PaymentQuery
{
    public record GetAllPaymentsQuery() : IRequest<List<PaymentDto>>;

    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, List<PaymentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllPaymentsQueryHandler> _logger;

        public GetAllPaymentsQueryHandler(IUnitOfWork unitOfWork, Logger<GetAllPaymentsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<List<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            var payments = await _unitOfWork.Payments.GetAllPayments(cancellationToken);
            if (payments == null || payments.Count == 0)
            {
                    _logger.LogInformation("No payments found in the database.");
                 return new List<PaymentDto>();
            }

            _logger.LogInformation("Retrieved {Count} payments from the database.", payments.Count);
            return payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                FailureReason = payment.FailureReason,
                PaymentReference = payment.PaymentReference,
                Provider = payment.Provider,
                TransactionId = payment.TransactionId
            }).ToList();
        }
    }

}
