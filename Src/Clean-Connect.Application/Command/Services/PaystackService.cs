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
using System.Threading.Tasks;

namespace Clean_Connect.Application.Command.Services
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _paystackSecretKey;

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

            var content = await response.Content.ReadFromJsonAsync<dynamic>();

            if (response.IsSuccessStatusCode && content.status == true)
            {
                return new PaystackInitResponse
                {
                    Status = true,
                    AuthorizationUrl = content.data.authorization_url,
                    Reference = content.data.reference
                };
            }
            else
            {
                throw new Exception($"Paystack initialization failed: {content.message}");
            }
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
    }
}