using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandServices _brandService;
        private readonly IAuthService _authService;

        public BrandController(IBrandServices brandService, IAuthService authService)
        {
            _brandService = brandService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {

                var brands = await _brandService.GetAllAsync();
                return Ok(brands);
        }

        [HttpGet("GetBrandID/{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {

                var brandModel = await _brandService.GetByIdAsync(id);
                return Ok(brandModel);
        }

        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm]BrandDto dto)
        {
 
                var userID = User.FindFirst("uid")?.Value;

                if (userID == null || !await _authService.isAdmin(userID))
                {
                    throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
                }

                BrandModel brand = new()
                {
                    Name = dto.Name,
                    ImageFiles = dto.ImageFile
                };

                var createdBrand = await _brandService.CreatedAsync(brand);
                return Ok(new { message = "the brand are added successfully", createdBrand });



        }

        [HttpDelete("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {


            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            if (id != 0)
            {
                 var message = await _brandService.DeleteBrand(id);
                 return Ok(message);

            }
            throw new BadRequestException("Can't find this brand");      


        }

        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditBrand(int id, [FromForm] BrandDto updateDto)
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            // Map DTO to BrandModel
            var updatedBrand = new BrandModel
                {
                    Name = updateDto.Name,
                    ImageFiles = updateDto.ImageFile
                };

                // Call the service
                var result = await _brandService.EditBrandAsync(id, updatedBrand);


                return Ok(new { message = "the brand are edited successfully", result });

        }

    }
    
}
