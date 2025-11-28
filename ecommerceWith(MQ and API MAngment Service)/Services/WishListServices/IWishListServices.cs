namespace MultiTenancy.Services.WishListServices
{
    public interface IWishListServices
    {
        Task<WishListModel> GetWishlistAsync(string userId);
        Task<WishListModel> AddToWishlistAsync(string userId, int productId);
        Task<WishListModel> RemoveFromWishlistAsync(string userId, int productId);
        Task<bool> ClearWishlistAsync(string userId);
        Task<List<ProductModel>> GetAllProductinWishList(string userId);

    }
}
