using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class WishListController : ControllerBase
    {
        private readonly IWishListServices _wishList;
        private readonly IAuthService _authService;

        public WishListController(IWishListServices wishList, IAuthService authService)
        {
            _wishList = wishList;
            _authService = authService;
        }
        [HttpGet("Product")]
        public async Task<IActionResult> GetWishlistProducts()
        {
            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

                var Products = await _wishList.GetAllProductinWishList(userId);
                return Ok(Products);

        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var wishlist = await _wishList.GetWishlistAsync(userId);
            return Ok(wishlist);

        }

        [HttpPost("add/{ProductId}")]
        public async Task<IActionResult> AddToWishlist(int ProductId)
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }


                var wishlist = await _wishList.AddToWishlistAsync(userId, ProductId);
                return Ok(new { message = "the product are added successfully", wishlist });

        }

        [HttpDelete("remove/{ProductId}")]
        public async Task<IActionResult> RemoveFromWishlist(int ProductId)
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

                var wishlist = await _wishList.RemoveFromWishlistAsync(userId, ProductId);
                return Ok(new { message = "the product are deleted successfully", wishlist });

        }

        // Clear Wishlist
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isUser(userId))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }

                var success = await _wishList.ClearWishlistAsync(userId);
                return success ? Ok(new { message = "Wishlist cleared successfully" }) : NotFound();
            
        }
    }
}
