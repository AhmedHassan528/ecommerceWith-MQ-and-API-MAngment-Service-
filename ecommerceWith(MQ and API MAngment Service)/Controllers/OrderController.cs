using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Services.OrderService;
using Stripe;
using Stripe.Checkout;
using static ecommerceWith_MQ_and_API_MAngment_Service_.Errors.CustomExceptions;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ApplicationDbContext context;
        private readonly IAuthService _authService;
        private readonly ISendMail _mail;



        public OrderController(IOrderService orderService, ApplicationDbContext context, IAuthService authService, ISendMail mail)
        {
            _orderService = orderService;
            this.context = context;
            _authService = authService;
            _mail = mail;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {

            var ReqUrl = Request.Headers["Origin"].ToString();

            var userID = User.FindFirst("uid")?.Value;
            var CustomerName = User.FindFirst("CustomerName")?.Value;
            if (CustomerName == null)
            {
                CustomerName = "Customer";
            }

            if (userID == null || !await _authService.isUser(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

                var (order, sessionId) = await _orderService.CreateOrderAsync(userID, request.CartId, request.addressId, ReqUrl, CustomerName);
                return Ok(new { orderId = order.Id, sessionId });
            

        }



        [HttpGet("verify-session/{sessionId}")]
        public async Task<IActionResult> VerifySession(string sessionId)
        {


            StripeConfiguration.ApiKey = "sk_test_51R23Ds4Fqtfdl7NOTiNaUMFRuZ619weBkBPRQFKr7Q1fRJq4CJoEEf5BoQJY5z32qRY7R8zANTz0LcgeON6VNBaI00xNQpiQ5U";

            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            if (session.Status == "complete" && session.PaymentStatus == "paid")
            {
                var order = await context.Orders
                    .FirstOrDefaultAsync(o => o.PaymentIntentId == session.Id );
                if (order != null)
                {
                    order.status = true;
                    context.Orders.Update(order);
                    await context.SaveChangesAsync();


                    await _mail.sendInvoice(order.Id);

                    return Ok(new { orderId = order.Id, status = "success" });
                }
            }
            throw new BadRequestException("Payment verification failed or order not found");
        }

        [HttpGet("GetUserOrders")]
        public async Task<IActionResult> GetUserOrders()
        {

            var userID = User.FindFirst("uid")?.Value;

            if (userID == null || !await _authService.isUser(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var orders = await _orderService.GetAllOrdersAsync(userID);
                return Ok(orders);

        }


        [HttpGet("AdminGetAllOrdersl")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminGetAllOrders()
        {

            var userID = User.FindFirst("uid")?.Value;
            if (userID == null || !await _authService.isAdmin(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

            var orders = await _orderService.AdminGetAllOrdersAsync(userID);
            return Ok(orders);
           
        }
        // order details
        [HttpGet("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var userID = User.FindFirst("uid")?.Value;

            if (userID == null || !await _authService.isAdmin(userID))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }

                var order = await _orderService.GetOrderAsync(orderId);
                return Ok(order);
           
        }

        [HttpPut("updateOrderStatus/{orderId}/{statusMass}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateOrderStatus(int orderId, string statusMass)
        {

            var userId = User.FindFirst("uid")?.Value;
            if (userId == null || !await _authService.isAdmin(userId))
            {
                throw new UnauthorizedException("Error: User not found. \nPlease ensure you have entered the correct username or email, or register for an account.");
            }


            var order = await _orderService.updateOrderStatus(userId, orderId, statusMass);
                if (string.IsNullOrEmpty(order))
                {
                    return Ok(order);
                }
            throw new BadRequestException("some thing error when getting order try again later!");

        }



    }
    public class CreateOrderRequest
    {
        public int CartId { get; set; }
        public int addressId { get; set; }

    }
}
