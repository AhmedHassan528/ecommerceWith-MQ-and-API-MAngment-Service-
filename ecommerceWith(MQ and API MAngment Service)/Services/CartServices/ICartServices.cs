namespace MultiTenancy.Services.CartServices
{
    public interface ICartServices
    {
        Task<CartModel> AddItemToCartAsync(string userId, int productId, int quantity);
        Task<CartModel> RemoveItemFromCartAsync(string userId, int productId);
        Task<CartModel> GetUserCartAsync(string userId);
        Task<CartModel> ClearCartAsync(string userId);
        Task<CartModel> IncreaseItemCountAsync(string userId, int productId);
        Task<CartModel> DecreaseItemCountAsync(string userId, int productId);



    }
}
