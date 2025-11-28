using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Authentication_With_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ISendMail _sendMail;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IAuthService authService, ISendMail sendMail, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _sendMail = sendMail;
            _userManager = userManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var ReqUrl = Request.Headers["Origin"].ToString();


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model, ReqUrl);

            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "You’ve got mail! Please check your inbox to confirm your email address." });

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(new { message = result.Message });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7),
                Domain = null
                               
            };

            Response.Cookies.Append("jwtToken", result.Token, cookieOptions);

            // Return only user info, not token
            return Ok(new
            {
                message = "Login successful",
                result.Username,
                result.Email,
                result.Roles
            });
        }

        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null || token == null)
            {
                return BadRequest("Link expired");

            }
            else if (user == null)
            {
                return BadRequest("User not Found");

            }
            var result = await _authService.ConfirmEmail(userId, token);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok();

        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var ReqUrl = Request.Headers["Origin"].ToString() + "/reset-password";

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            else
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _sendMail.SendEmailAsync(email, "Reset Password", token, "ForgotPasswordConfermation", ReqUrl);
                return Ok(result);
            }
        }
        [HttpPost("ForgotPasswordConfermation")]
        public async Task<IActionResult> ForgotPasswordConfermation([FromBody] ForgotPasswordConfermationModel model)
        {

            if (model.newPassword != model.confirmPassword)
            {
                return BadRequest("Password not match");
            }

            var result = await _authService.ForgotPasswordConfermationModel(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok();

        }




        [HttpPost("AddRoleToUser")]
        [Authorize]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddRoleToUser([FromHeader] string userEmail)
        {
            try
            {

                var AdminID = User.FindFirst("uid")?.Value;
                if (AdminID == null || userEmail == null)
                {
                    return BadRequest("User not found");
                }
                var result = await _authService.setAdminRole(AdminID, userEmail);

                return Ok(new { message = result });

            }
            catch (Exception ex)
            {


                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet("GetAllUsersAsync")]
        [Authorize]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAsync()
        {

            var AdminID = User.FindFirst("uid")?.Value;
            if (AdminID == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _authService.GetAllUsersAsync(AdminID);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("CheckAuth")]
        public IActionResult CheckAuth()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Ok(new { isAuthenticated = true });

            return Ok(new { isAuthenticated = false });
        }

        [HttpGet("CheckAdmin")]
        [Authorize]
        public async Task<IActionResult> CheckAdmin()
        {

            if (User.Identity != null && User.Identity.IsAuthenticated && await _authService.isAdmin(User.FindFirst("uid")?.Value))
            {

                return Ok(new { isAuthenticated = true });
            }

            return BadRequest(new { isAuthenticated = false });
        }


    }
}