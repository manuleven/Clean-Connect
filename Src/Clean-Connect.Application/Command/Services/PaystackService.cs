using Clean_Connect.Application.DTO;
using Clean_Connect.Application.Interface.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _paystackSecretKey;
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public PaystackService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _paystackSecretKey = config["Paystack:SecretKey"] ?? throw new ArgumentNullException("Paystack secret key is not configured.");
        }

        public async Task<PaystackInitResponse> InitializePayment(decimal amount, string email, string reference)
        {
            var requestData = new
            {
                email,
                amount = (int)(amount * 100), // Paystack expects amount in kobo
                reference
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackSecretKey);

            var response = await _httpClient.PostAsJsonAsync("https://api.paystack.co/transaction/initialize", requestData);

            var content = await response.Content.ReadAsStringAsync();
            var paystackResponse = JsonSerializer.Deserialize<PaystackInitializeApiResponse>(content, JsonOptions);

            if (response.IsSuccessStatusCode && paystackResponse?.Status == true && paystackResponse.Data != null)
            {
                return new PaystackInitResponse
                {
                    Status = true,
                    AuthorizationUrl = paystackResponse.Data.AuthorizationUrl,
                    Reference = paystackResponse.Data.Reference
                };
            }

            throw new Exception($"Paystack initialization failed: {paystackResponse?.Message ?? content}");
        }

        public async Task<PaystackVerifyResponse> VerifyTransaction(string reference)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _paystackSecretKey);

            var response = await _httpClient.GetAsync(
                $"https://api.paystack.co/transaction/verify/{reference}"
            );

            response.EnsureSuccessStatusCode();

            var content = await response.Content
                .ReadFromJsonAsync<PaystackVerifyResponse>();

            if (content == null)
            {
                throw new Exception("Unable to verify transaction");
            }

            return content;
        }

        private sealed record PaystackInitializeApiResponse(
            bool Status,
            string? Message,
            PaystackInitializeData? Data);

        private sealed record PaystackInitializeData(
            [property: JsonPropertyName("authorization_url")] string AuthorizationUrl,
            string Reference);
    }
}
