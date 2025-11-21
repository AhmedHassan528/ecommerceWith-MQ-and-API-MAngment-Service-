using MultiTenancy.Dtos.PaymobDtos;
using Newtonsoft.Json;
using System.Text;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Services.paymobServices
{
    public class PaymobService : IPaymobService
    {
        private readonly HttpClient _httpClient;

        public PaymobService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AuthenticateAsync()
        {
            var authRequest = new AuthRequest
            {
                api_key = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2T0RFMU56QTBMQ0p1WVcxbElqb2lhVzVwZEdsaGJDSjkuNjVreFE3SHdtYjhLQTFNMzY1U1gwYkFHQXcyV2xrMUJGd0poVnpBZ0R5QVpsbVpiTm5TUW15WFZ3dk0tMGRQc0dJaU9qYUE4UkdkSlFDcjNWSTF2eFE="
            };

            var content = new StringContent(JsonConvert.SerializeObject(authRequest.api_key), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://accept.paymobsolutions.com/api/auth/tokens", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to authenticate with Paymob");
            }

            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(await response.Content.ReadAsStringAsync());
            return authResponse.token;
        }

        public async Task<int> CreateOrderAsync(string authToken, decimal amountCents)
        {
            var orderRequest = new OrderRequest
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = amountCents,
                currency = "EGP",
                items = new List<object>()
            };

            var content = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://accept.paymobsolutions.com/api/ecommerce/orders", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new BadRequestException("Failed to create order with Paymob");
            }

            var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(await response.Content.ReadAsStringAsync());
            return orderResponse.id;
        }

        public async Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amountCents, BillingData billingData)
        {
            var paymentKeyRequest = new PaymentKeyRequest
            {
                auth_token = authToken,
                amount_cents = amountCents,
                expiration = 3600,
                order_id = orderId,
                billing_data = billingData,
                currency = "EGP",
                integration_id = 3892419
            };

            var content = new StringContent(JsonConvert.SerializeObject(paymentKeyRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://accept.paymobsolutions.com/api/acceptance/payment_keys", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to get payment key from Paymob");
            }

            var paymentKeyResponse = JsonConvert.DeserializeObject<PaymentKeyResponse>(await response.Content.ReadAsStringAsync());
            return paymentKeyResponse.token;
        }
    }
}
