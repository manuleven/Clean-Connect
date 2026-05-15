using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Clean_Connect.Application.DTO
{

    public record WorkerBankAccountDto(
        string AccountNumber,
        string BankCode,
        string AccountName,
        string Currency = "NGN");

    public class TransferRecipientResponse
    {
        [JsonPropertyName("recipient_code")]
        public string RecipientCode { get; set; } = default!;

        [JsonPropertyName("domain")]
        public string? Domain { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }

    public class TransferInitiationResponse
    {
        [JsonPropertyName("transfer_code")]
        public string TransferCode { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }
    }

    public record PayoutResult(bool Success, string Message, string? ProviderReference = null);
}
