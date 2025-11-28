using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {

        private readonly ICartServices _cartServices;
        private readonly IAuthService _authService;


        public CartController(ICartServices cartServices, IAuthService authService)
        {
            _cartServices = cartServices;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var cart = await _cartServices.GetUserCartAsync(userId);
            return Ok(cart);

        }

        [HttpPost("add/{ProductId}")]
        public async Task<IActionResult> AddItemToCart(int ProductId)
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

           var cart = await _cartServices.AddItemToCartAsync(userId, ProductId, 1);
           return Ok(new { message = "the item are added successfully", cart });


        }

        [HttpDelete("Remove/{ProductId}")]
        public async Task<IActionResult> RemoveItemFromCart(int ProductId)
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var cart = await _cartServices.RemoveItemFromCartAsync(userId, ProductId);
            return Ok(new { message = "the item are removed successfully", cart });

        }

        [HttpPut("increase/{ProductId}")]
        public async Task<IActionResult> IncreaseItemCount( int ProductId)
        {

            var UserId = User.FindFirst("uid")?.Value;
            if (UserId == null || !await _authService.isUser(UserId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var cart = await _cartServices.IncreaseItemCountAsync(UserId, ProductId);
            return Ok(new { message = "The number of products was successfully increased.", cart });

            
        }

        [HttpPut("decrease/{ProductId}")]
        public async Task<IActionResult> DecreaseItemCount(int ProductId)
        {

            var UserId = User.FindFirst("uid")?.Value;
            if (UserId == null || !await _authService.isUser(UserId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var cart = await _cartServices.DecreaseItemCountAsync(UserId, ProductId);
            return Ok(new { message = "The number of products was successfully decreased.", cart });



        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var cart = await _cartServices.ClearCartAsync(userId);
                return Ok(new { message = "Cart cleared successfully", cart });
            }
        }

    
}
