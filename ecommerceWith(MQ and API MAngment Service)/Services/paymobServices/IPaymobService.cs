using MultiTenancy.Dtos.PaymobDtos;

namespace MultiTenancy.Services.paymobServices
{
    public interface IPaymobService
    {
        Task<string> AuthenticateAsync();
        Task<int> CreateOrderAsync(string authToken, decimal amountCents);
        Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amountCents, BillingData billingData);
    }
}
