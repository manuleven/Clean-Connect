using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clean_Connect.Application.Query.PaymentQuery
{
    public record GetPaymentByIdCQuery(Guid PaymentId) : IRequest<PaymentDto>;

    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdCQuery, PaymentDto>
    {
        private readonly IUnitOfWork repo;
        private readonly ILogger<GetPaymentByIdQueryHandler> logger;

        public GetPaymentByIdQueryHandler(IUnitOfWork repo, ILogger<GetPaymentByIdQueryHandler> logger)
        {
            this.repo = repo;
            this.logger = logger;
        }

        public async Task<PaymentDto> Handle(GetPaymentByIdCQuery request, CancellationToken cancellationToken)
        {
            if (request.PaymentId == Guid.Empty)
                throw new ArgumentNullException(nameof(request.PaymentId));

            var payment = await repo.Payments.GetPaymentById(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                logger.LogWarning("Payment with ID {PaymentId} not found.", request.PaymentId);
                throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");
            }

            var paymentResult = new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                FailureReason = payment.FailureReason,
                PaymentReference = payment.PaymentReference,
                Provider = payment.Provider,
                TransactionId = payment.TransactionId,
                
            };

            

            logger.LogInformation("Payment with ID {PaymentId} retrieved successfully.", request.PaymentId);
            return paymentResult;




        }
    }
}
