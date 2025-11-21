namespace MultiTenancy.Services.CategoriesServices
{
    public interface ICategoriesServices
    {
        Task<CategoryModel> CreatedAsync(CategoryModel category);
        Task<CategoryModel?> GetByIdAsync(int id);
        Task<string> DeleteCategory(int id);
        Task<IReadOnlyList<CategoryModel>> GetAllAsync();

        Task<CategoryModel> EditCategoryAsync(int id, CategoryModel updatedCategory);
    }
}
