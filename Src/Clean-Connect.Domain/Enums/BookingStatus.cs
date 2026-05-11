namespace Clean_Connect.Domain.Enums
{
    public enum BookingStatus
    {
        Pending,
        AcceptedAwaitingPayment,
        MarkAsPaid,
        InProgress,
        AwaitingClientConfirmation,
        Rejected,
        Cancelled,
        Completed
    }
}
