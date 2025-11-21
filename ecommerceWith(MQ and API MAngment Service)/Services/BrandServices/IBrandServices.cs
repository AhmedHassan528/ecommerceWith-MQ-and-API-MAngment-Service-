namespace MultiTenancy.Services.BrandServices
{
    public interface IBrandServices
    {
        Task<BrandModel> CreatedAsync(BrandModel brand);
        Task<BrandModel?> GetByIdAsync(int id);
        Task<string> DeleteBrand(int id);
        Task<IReadOnlyList<BrandModel>> GetAllAsync();

        Task<BrandModel> EditBrandAsync(int id, BrandModel updatedBrand);
    }
}
