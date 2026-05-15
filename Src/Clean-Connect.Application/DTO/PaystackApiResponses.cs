using System.Text.Json.Serialization;

namespace Clean_Connect.Application.DTO
{
    public sealed record PaystackInitializeApiResponse(
        bool Status,
        string? Message,
        PaystackInitializeData? Data);

    public sealed record PaystackInitializeData(
        [property: JsonPropertyName("authorization_url")] string AuthorizationUrl,
        string Reference);

    public sealed record PaystackTransferRecipientResponse(
        bool Status,
        string? Message,
        TransferRecipientResponse? Data);

    public sealed record PaystackTransferResponse(
        bool Status,
        string? Message,
        TransferInitiationResponse? Data);
}
