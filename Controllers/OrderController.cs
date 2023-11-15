using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TazaFood_API.DTO;
using TazaFood_Core.Models.Order_Aggregate;
using TazaFood_Core.Services;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderservice;
        private readonly IMapper mapper;

        public OrderController(IOrderService Orderservice,IMapper Mapper)
        {
            orderservice = Orderservice;
            mapper = Mapper;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> createOrder(OrderDto newOrder) 
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var address = mapper.Map<UserAddressDto, Address>(newOrder.ShippingAddress);
            var order = await orderservice.CreateOrder(email, newOrder.cartId, newOrder.DeliveryMethod, address);
            if (order is null) return BadRequest("cant add order");
            return Ok(mapper.Map<Order,OrderReturnToDto>(order));
        }
       
        [Authorize]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(string UserEmail)
        {
            var Email = User.FindFirstValue(ClaimTypes.Email);
            var orders = orderservice.GetAllOrdersForUser(Email);
            return Ok(mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderReturnToDto>>(orders.Result));
        }


        [Authorize]
        [HttpGet("GetOrder")]
        public async Task<IActionResult> getUserOrder(int orderId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var order = await orderservice.GetOrderById(orderId, email);
            if (order is null) return NotFound();
            return Ok(mapper.Map<Order, OrderReturnToDto>(order));
        }


        [Authorize]
        [HttpGet("DeliveryMethods")]
        public async Task<IActionResult> GetAllDeleveryMethods()
        {
            var deleverymethods = await orderservice.GetDeleveryMethods();
            return Ok(deleverymethods);
        }
    }

}
