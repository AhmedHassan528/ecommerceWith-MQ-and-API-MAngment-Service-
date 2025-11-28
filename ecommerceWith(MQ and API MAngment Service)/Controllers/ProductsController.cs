using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IAuthService _authService;



    public ProductsController(IProductService productService, IAuthService authService)
    {
        _productService = productService;
        _authService = authService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {


            var products = await _productService.GetAllAsync();
            var productDtos = products.Select(p => new ProductsDtos
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                RatingsQuantity = p.RatingsQuantity,
                ImageCover = p.ImageCover,
                CategoryName = p.Category?.Name,
                BrandName = p.Brand?.Name
            }).ToList();

            return Ok(productDtos);



    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAsync(int id)
    {


            if (id <= 0)
            {
                throw new BadRequestException("Invalid product ID");
            }
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                throw new NotFoundException("can not find this product");
            }

            var productDto = new ProductsDtos
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                RatingsQuantity = product.RatingsQuantity,
                Images = product.Images,
                CategoryID = product.CategoryID,
                CategoryName = product.Category?.Name,
                BrandName = product.Brand?.Name,
                BrandID = product.BrandID
            };

            return Ok(productDto);
        
    }

    [Authorize]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatedAsync([FromForm] CreateProductDto dto)
    {


        var userID = User.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userID) || !await _authService.isAdmin(userID))
        {
            throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
        }

            ProductModel product = new()
            {
                NumSold = dto.NumSold,
                ImageFiles = dto.ImageFiles,
                RatingsQuantity = dto.ratingsQuantity,
                Title = dto.title,
                Description = dto.description,
                Price = dto.price,
                ImageCoverFile = dto.ImageCoverFile,
                CategoryID = dto.CategoryID,
                BrandID = dto.BrandID
            };

            var createdProduct = await _productService.CreatedAsync(product);

            return Ok(new { message = "Product created successfully!",createdProduct});

        

        
    }

    [HttpDelete("{id}")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {

        var userID = User.FindFirst("uid")?.Value;
        if (userID == null || !await _authService.isAdmin(userID))
        {
            throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
        }


        if (id <= 0)
        {
            throw new BadRequestException("Invalid product ID");
        }

            var result = await _productService.DeleteProduct(id);
            return Ok(result);

    }

    [HttpPut("{id}")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductModel productModel)
    {

        var userID = User.FindFirst("uid")?.Value;
        if (userID == null || !await _authService.isAdmin(userID))
        {
            throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
        }

        if (!ModelState.IsValid)
        {
            throw new BadRequestException("Invalid product data");
        }

            var updatedProduct = await _productService.UpdateProductAsync(
                id,
                productModel,
                productModel.ImageCoverFile,
                productModel.ImageFiles ?? new List<IFormFile>()
            );
            return Ok(new { message = "Product Updated successfully!",updatedProduct});

        

    }

    [HttpGet("AdminGetAllAsync")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGetAllAsync()
    {

        var userID = User.FindFirst("uid")?.Value;
        if (userID == null )
        {
            throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
        }


        var products = await _productService.GetAllAsync();
        return Ok(products);

    }
}

