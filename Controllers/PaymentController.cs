using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using TazaFood_Core.Models.Order_Aggregate;
using TazaFood_Core.Services;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        private readonly ILogger<PaymentController> logger;
        private const string endpointSecret = "whsec_123";

        public PaymentController(IPaymentService PaymentService, ILogger<PaymentController> Logger)
        {
            paymentService = PaymentService;
            logger = Logger;
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


        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
           
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], endpointSecret);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
            Order order;

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await paymentService.UpdatePaymentIntentWithSucceededOrFaild(paymentIntent.Id, true);
                    logger.LogInformation("Payment Intent Is Succeeded :)", paymentIntent.Id);
                    break;
                case Events.PaymentIntentPaymentFailed:
                    order = await paymentService.UpdatePaymentIntentWithSucceededOrFaild(paymentIntent.Id, false);
                    logger.LogInformation("Payment Intent Is Failed :( ", paymentIntent.Id);

                    break;
                default:
                    break;
            }

            return Ok("done");
           
        }
    }
}
