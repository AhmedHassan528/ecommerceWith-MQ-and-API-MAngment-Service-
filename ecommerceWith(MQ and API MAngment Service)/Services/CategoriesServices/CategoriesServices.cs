
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Services.CategoriesServices
{
    public class CategoriesServices : ICategoriesServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hosting;
        private readonly string _imageStoragePath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoriesServices(ApplicationDbContext context, IWebHostEnvironment hosting, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.hosting = hosting;

            _imageStoragePath = Path.Combine(hosting.WebRootPath, "CategoryImages");
            Directory.CreateDirectory(_imageStoragePath);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CategoryModel> CreatedAsync(CategoryModel category)
        {
            string imagePath = "";
            try
            {

                if (category.ImageFiles != null)
                {
                    string ImageFolder = Path.Combine(hosting.WebRootPath, "CategoryImages");

                    string fileExtension = Path.GetExtension(category.ImageFiles.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    imagePath = Path.Combine(ImageFolder, fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        category.ImageFiles.CopyTo(stream);
                    }

                    var request = _httpContextAccessor.HttpContext?.Request;
                    string baseUrl = $"{request?.Scheme}://{request?.Host}";

                    category.Image = $"{baseUrl}/CategoryImages/{fileName}";
                }
                else
                {
                    throw new BadRequestException("Must add cover Image");
                }


                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception)
            {
                File.Delete(imagePath!);
                throw;

            }
        }

        public async Task<string> DeleteCategory(int id)
        {

                var category = await _context.Categories.FindAsync(id);
                if (category != null)
                {
                    var products = await _context.Products.Where(p => p.CategoryID == id).ToListAsync();
                    foreach (var product in products)
                    {
                        if (!string.IsNullOrEmpty(product.ImageCover))
                        {
                            var imagePath = Path.Combine(_imageStoragePath, Path.GetFileName(product.ImageCover));
                            if (File.Exists(imagePath))
                            {
                                File.Delete(imagePath);
                            }
                        }
                        if (product.Images != null)
                        {
                            foreach (var image in product.Images)
                            {
                                var imagePath = Path.Combine(_imageStoragePath, Path.GetFileName(image));
                                if (File.Exists(imagePath))
                                {
                                    File.Delete(imagePath);
                                }
                            }
                        }
                        var cartItems = await _context.CartItemes.Where(ci => ci.ProductId == product.Id).ToListAsync();
                        _context.CartItemes.RemoveRange(cartItems);
                        _context.Products.Remove(product);
                    }
                    _context.Categories.Remove(category);

                    await _context.SaveChangesAsync();
                    return "The category and its related products have been deleted";
                }
                throw new NotFoundException("Can't find this category");
            

        }

        public async Task<IReadOnlyList<CategoryModel>> GetAllAsync()
        {

                return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {

                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    throw new NotFoundException("can not find category!!");
                }
                return category;
        }


        public async Task<CategoryModel> EditCategoryAsync(int id, CategoryModel updatedCategory)
        {


                // Find the existing brand
                var category = await _context.Categories
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (category == null)
                {
                    throw new NotFoundException("Brand not found");
                }

                // Update name if provided
                if (!string.IsNullOrWhiteSpace(updatedCategory.Name))
                {
                    category.Name = updatedCategory.Name;
                }

                // Handle image upload if provided
                if (updatedCategory.ImageFiles != null && updatedCategory.ImageFiles.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(updatedCategory.ImageFiles.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        throw new BadRequestException("Invalid image format. Only JPG, JPEG, PNG, and GIF are allowed.");
                    }

                    // Generate unique file name
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(_imageStoragePath, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updatedCategory.ImageFiles.CopyToAsync(stream);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(category.Image))
                    {
                        var oldImagePath = Path.Combine(_imageStoragePath, Path.GetFileName(category.Image));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    // Update image path
                    category.Image = $"https://localhost:7060/CategoryImages/{fileName}"; // Updated path
                }

                // Save changes
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return category;

        }
    }
}
