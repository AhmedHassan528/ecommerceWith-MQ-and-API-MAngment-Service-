using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiTenancy.Dtos.PaymobDtos;
using MultiTenancy.Services.paymobServices;

namespace MultiTenancy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly IPaymobService _paymobService;

        public PaymobController(IPaymobService paymobService)
        {
            _paymobService = paymobService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequest request)
        {

                // Step 1: Authenticate with Paymob
                var authToken = await _paymobService.AuthenticateAsync();

                // Step 2: Create an order
                var amountCents = request.Amount * 100; // Convert to cents
                var orderId = await _paymobService.CreateOrderAsync(authToken, amountCents);

                // Step 3: Get payment key
                var billingData = new BillingData
                {
                    email = request.Email,
                    first_name = request.FirstName,
                    last_name = request.LastName,
                    phone_number = request.PhoneNumber,
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "NA",
                    country = "NA",
                    state = "NA"
                };

                var paymentKey = await _paymobService.GetPaymentKeyAsync(authToken, orderId, amountCents, billingData);

                // Step 4: Construct iframe URL
                var iframeUrl = $"https://accept.paymobsolutions.com/api/acceptance/iframes/819217?payment_token={paymentKey}";

                return Ok(new { IframeUrl = iframeUrl, OrderId = orderId });
            
        }

        [HttpPost("callback")]
        public IActionResult PaymentCallback([FromQuery] PaymobCallback callback)
        {
            // Verify the callback (e.g., check HMAC signature)
            // Update order status in your database
            // Example: Log the callback for now
            Console.WriteLine($"Payment Callback: OrderId={callback.order}, Success={callback.success}, Amount={callback.amount_cents}");

            return Ok();
        }

        public class PaymentRequest
        {
            public decimal Amount { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PhoneNumber { get; set; }
        }

        public class PaymobCallback
        {
            public string obj { get; set; }
            public int order { get; set; }
            public bool success { get; set; }
            public decimal amount_cents { get; set; }
            public string hmac { get; set; }
        }
    }
}
