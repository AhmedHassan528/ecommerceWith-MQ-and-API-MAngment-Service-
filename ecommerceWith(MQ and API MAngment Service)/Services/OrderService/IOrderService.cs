

namespace MultiTenancy.Services.OrderService
{
    public interface IOrderService
    {
        Task<(Order order, string clientSecret)> CreateOrderAsync(string userID, int cartId, int addressID, string HostUrl, string CustomerName);
        Task<Queue<Order>> GetAllOrdersAsync(string userID);


        Task<Order> GetOrderAsync(int orderId);
        Task<Queue<Order>> AdminGetAllOrdersAsync(string userID);
        Task<string> updateOrderStatus(string userID, int orderID, string statusMass);

    }
}
