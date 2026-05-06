using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean_Connect.Domain.Events
{
    public sealed class PaymentCreatedEvent : DomainEvent
    {
        public Guid PaymentId { get; }

        public PaymentCreatedEvent(Guid paymentId)
        {
            PaymentId = paymentId;
        }

    }

    public sealed class PaymentFailedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public string FailureReason { get; }
        public PaymentFailedEvent(Guid paymentId, string failureReason)
        {
            PaymentId = paymentId;
            FailureReason = failureReason;
        }
    }

    public sealed class PaymentRefundedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public decimal RefundAmount { get; }
        public string Reason { get; }
        public PaymentRefundedEvent(Guid paymentId, decimal refundAmount, string reason)
        {
            PaymentId = paymentId;
            RefundAmount = refundAmount;
            Reason = reason;
        }
    }

    public sealed class PaymentCancelledEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public string CancellationReason { get; }
        public PaymentCancelledEvent(Guid paymentId, string cancellationReason)
        {
            PaymentId = paymentId;
            CancellationReason = cancellationReason;
        }
    }

    public sealed class PaymentRefundFailedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public string FailureReason { get; }
        public PaymentRefundFailedEvent(Guid paymentId, string failureReason)
        {
            PaymentId = paymentId;
            FailureReason = failureReason;
        }
    }
    public sealed class PaymentSuccessfulEvent : DomainEvent
    {
        public Guid PaymentId { get; }

        public PaymentSuccessfulEvent(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }

    public sealed class PaymentAbandonedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public string Reason { get; }

        public PaymentAbandonedEvent(Guid paymentId, string reason)
        {
            PaymentId = paymentId;
            Reason = reason;
        }
    }


}
