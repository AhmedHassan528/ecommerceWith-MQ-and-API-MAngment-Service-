namespace MultiTenancy.Services.ProductsServices;

public interface IProductService
{
    Task<ProductModel> CreatedAsync(ProductModel product);
    Task<ProductModel?> GetByIdAsync(int id);
    Task<string> DeleteProduct(int id);
    Task<IReadOnlyList<ProductModel>> GetAllAsync();
    Task<ProductModel> UpdateProductAsync(int id, ProductModel productModel, IFormFile imageCoverFile, List<IFormFile> imageFiles);

}
 
