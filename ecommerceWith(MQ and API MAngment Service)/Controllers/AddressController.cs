using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressServices _addressServices;
        private readonly IAuthService _authService;

        public AddressController(IAddressServices addressServices, IAuthService authService)
        {
            _addressServices = addressServices;
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isUser(userID))
            {

                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }
            var Addresses = await _addressServices.GetUserAddresses(userID);
            return Ok(Addresses);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddresesesDto address)
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.", StatusCode = 401 });
            }
            var addresses = await _addressServices.AddAddress(userID, address);

            return Ok(new { message = "the Address are added successfully", addresses });

        }

        [HttpGet("GetAddressByID/{addressID}")]
        public async Task<IActionResult> GetAddressByID(int addressID)
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account." });
            }
            var Address = await _addressServices.GetAddressByID(userID, addressID);
            return Ok(Address);
        }

        [HttpDelete("{addressID}")]
        public async Task<IActionResult> DeleteAddressByID(int addressID)
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || addressID == 0 || !await _authService.isUser(userID))
            {
                return NotFound(new { message = "Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account." });

            }

            await _addressServices.DeleteAddressByID(userID, addressID);

            return Ok(new { message = "the Address are deleted successfully"});
        }




    }
}
