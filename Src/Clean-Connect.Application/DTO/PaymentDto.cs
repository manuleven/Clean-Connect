namespace Clean_Connect.Application.DTO
{
    public record ClientPaymentRequest(Guid ClientId, string Email, string PaymentMethod, string? CreatedBy = null);

    public record PaymentInitializationResponse(
        Guid BookingId,
        decimal Amount,
        string PaymentReference,
        string CheckoutUrl);
}
