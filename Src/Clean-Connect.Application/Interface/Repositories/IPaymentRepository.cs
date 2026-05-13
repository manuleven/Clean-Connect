using Clean_Connect.Domain.Entities;
using Clean_Connect.Domain.Enums;

namespace Clean_Connect.Application.Interface.Repositories
{
    public interface IPaymentRepository
    {
        Task CreatePayment(Payment payment, CancellationToken cancellationToken);
        Task DeletePayment(Guid paymentId, CancellationToken cancellationToken);
        Task<bool> ExistsByReferenceAsync(string reference, CancellationToken cancellationToken);
        Task<List<Payment>> GetAllPayments(CancellationToken cancellationToken);
        Task<Payment?> GetByReferenceAsync(string paymentReference, CancellationToken cancellationToken);
        Task<List<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken);
        Task<Payment> GetPaymentById(Guid paymentId, CancellationToken cancellationToken);
        Task<List<Payment?>> GetPaymentsByBookingId(Guid bookingId, CancellationToken cancellationToken);
        Task UpdatePayment(Payment payment, CancellationToken cancellationToken);
    }
}
