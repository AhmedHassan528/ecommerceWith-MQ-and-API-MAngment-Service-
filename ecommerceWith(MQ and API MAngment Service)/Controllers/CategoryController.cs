using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;


namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesServices _categoriesServices;
        private readonly IAuthService _authService;


        public CategoryController(ICategoriesServices categoriesServices, IAuthService authService)
        {
            _categoriesServices = categoriesServices;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
                var Categories = await _categoriesServices.GetAllAsync();
                return Ok(Categories);

            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {


                if (id == 0)
                {
                    throw new BadRequestException("can not find category!!");
                }
                var categoryModel = await _categoriesServices.GetByIdAsync(id);
                return Ok(categoryModel);
            

        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromForm]CreateCategoryDto dto)
        {


            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            CategoryModel category = new()
                {
                    Name = dto.Name,
                    ImageFiles = dto.ImageFiles
                };
            var createdProduct = await _categoriesServices.CreatedAsync(category);
            return Ok(new { message = "the Product are added successfully", createdProduct });




        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }


            if (id == 0)
            {
                throw new BadRequestException("Can't find this brand");
            }
            var message = await _categoriesServices.DeleteCategory(id);
            return Ok(message);
        }


        [HttpPut("{id}")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCategory(int id, [FromForm] CategoryDto updateDto)
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null)
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var updatedCategory = new CategoryModel
                {
                    Name = updateDto.Name,
                    ImageFiles = updateDto.ImageFiles
                };

                var result = await _categoriesServices.EditCategoryAsync(id, updatedCategory);
                return Ok(new { message = "the Product are edited successfully", result });



        }
    }
}
