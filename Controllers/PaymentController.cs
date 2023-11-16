using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TazaFood_Core.Services;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService PaymentService)
        {
            paymentService = PaymentService;
        }

        [Authorize]
        [HttpPost("CreatePayment")]
        public async Task<IActionResult> CreteOrUpdatePaymentIntent()
        {
            var usercartId = User.FindFirstValue(ClaimTypes.Email);
            var cart = await paymentService.CreateOrUpdatePaymentIntent(usercartId);
            if (cart is null) return BadRequest("OoPs there is not cart..");
            return Ok(cart);
        }
    }
}
