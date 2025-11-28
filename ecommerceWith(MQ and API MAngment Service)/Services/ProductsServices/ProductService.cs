using Microsoft.EntityFrameworkCore;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Services.ProductsServices;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment hosting;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductService(ApplicationDbContext context, IWebHostEnvironment hosting, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        this.hosting = hosting;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ProductModel> CreatedAsync(ProductModel product)
    {
        List<string> savedFilePaths = new List<string>();

        try
        {
            if (product.ImageCoverFile != null)
            {
                var coverFilePath = await SaveFileAsync(product.ImageCoverFile, "ProductCoverImages");
                product.ImageCover = coverFilePath;
            }
            else
            {
                throw new BadRequestException("Must add cover Image");
            }

            string imageFolder = Path.Combine(hosting.WebRootPath, "ProductImages");

            List<string> imageUrls = new List<string>();

            if (product.ImageFiles != null && product.ImageFiles.Count > 0)
            {
                foreach (var file in product.ImageFiles)
                {
                    var filePath = await SaveFileAsync(file, "ProductImages");
                    imageUrls.Add(filePath);
                }
            }
            product.Images = imageUrls;


            var category = await _context.Categories.FindAsync(product.CategoryID);
            var brand = await _context.Brands.FindAsync(product.BrandID);

            if (category == null || brand == null)
            {
                throw new BadRequestException("Invalid Category or Brand");
            }

            product.Category = category;
            product.Brand = brand;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
        catch (Exception)
        {
            foreach (var filePath in savedFilePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            throw;
        }
    }

    public async Task<string> DeleteProduct(int id)
    {

            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                throw new NotFoundException("Cannot find this product");
            }

            if (product.Images != null && product.Images.Count > 0)
            {
                foreach (var imageUrl in product.Images)
                {
                    string fileName = Path.GetFileName(imageUrl);
                    string imagePath = Path.Combine(hosting.WebRootPath, "ProductImages", fileName);

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
            }

            if (product.ImageCover != null)
            {
                string fileName = Path.GetFileName(product.ImageCover);
                string imagePath = Path.Combine(hosting.WebRootPath, "ProductCoverImages", fileName);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            var cartItems = await _context.CartItemes
                    .Where(ci => ci.ProductId == id)
                    .ToListAsync();

            if (cartItems.Any())
            {
                _context.CartItemes.RemoveRange(cartItems);

                // Update the TotalCartPrice and UpdatedAt for affected carts
                var affectedCartIds = cartItems.Select(ci => ci.CartId).Distinct().ToList();
                var affectedCarts = await _context.Carts
                    .Where(c => affectedCartIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var cart in affectedCarts)
                {
                    // Recalculate TotalCartPrice based on remaining items
                    var remainingItems = await _context.CartItemes
                        .Where(ci => ci.CartId == cart.Id)
                        .ToListAsync();

                    cart.TotalCartPrice = remainingItems.Sum(ci => ci.Price * ci.Count);
                    cart.UpdatedAt = DateTime.UtcNow;
                }
            }

            // Delete the product
            _context.Products.Remove(product);

            // Save all changes
            await _context.SaveChangesAsync();

            return "The product have been deleted successfully";
        


    }

    public async Task<IReadOnlyList<ProductModel>> GetAllAsync()
    {

            return await _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Brand).ToListAsync();

    }

    public async Task<ProductModel?> GetByIdAsync(int id)
    {

            var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("can not find this product");

            product.ViewCount++;
            await _context.SaveChangesAsync();

            return product;
    }

    public async Task<ProductModel> UpdateProductAsync(int id, ProductModel productModel, IFormFile imageCoverFile, List<IFormFile> imageFiles)
    {
        try
        {

            var existingProduct = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
            {
                throw new NotFoundException("Product not found");
            }

            // Update scalar properties
            existingProduct.NumSold = productModel.NumSold;
            existingProduct.RatingsQuantity = productModel.RatingsQuantity;
            existingProduct.Title = productModel.Title;
            existingProduct.Description = productModel.Description;
            existingProduct.Price = productModel.Price;
            existingProduct.ViewCount = productModel.ViewCount;
            existingProduct.CategoryID = productModel.CategoryID;
            existingProduct.BrandID = productModel.BrandID;

            // Handle ImageCoverFile
            if (imageCoverFile != null && imageCoverFile.Length > 0)
            {
                var coverFilePath = await SaveFileAsync(imageCoverFile, "ProductCoverImages");
                existingProduct.ImageCover = coverFilePath;
            }

            // Handle ImageFiles
            if (imageFiles != null && imageFiles.Any())
            {
                var imagePaths = new List<string>();
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        var filePath = await SaveFileAsync(file, "ProductImages");
                        imagePaths.Add(filePath);
                    }
                }
                existingProduct.Images = imagePaths;
            }

            _context.Entry(existingProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return existingProduct;
        }
        catch (Exception ex)
        {
            throw ;
        }

    }

    private async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0) throw new ArgumentException("File is empty", nameof(file));

        var uploadsFolder = Path.Combine(hosting.WebRootPath, folder);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        var FinalPath = Path.Combine(folder, fileName).Replace("\\", "/");
        // Return relative path for storage

        var request = _httpContextAccessor.HttpContext?.Request;
        string baseUrl = $"{request?.Scheme}://{request?.Host}";

        return $"{baseUrl}/{FinalPath}";
    }
}


