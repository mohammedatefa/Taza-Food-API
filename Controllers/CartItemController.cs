using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TazaFood_Core.IRepositories;
using TazaFood_Core.Models;

namespace TazaFood_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemsRepository cartItemRepo;

        public CartItemController(ICartItemsRepository _cartItemRepo)
        {
            cartItemRepo = _cartItemRepo;
        }

        //get cartitems by id
        [HttpGet("{id}")]
        [Route("GetCart")]
        public async Task<ActionResult<UserCart>> GetCart(string id) {
            var cart = await cartItemRepo.GetCartAsync(id);

            //if the cart is existed return it else create new one and return it 
            if(cart is null)
                return  new UserCart(id);
            return cart;
        }

        [HttpPost("UbdateOrCreate")]
        public async Task<ActionResult<UserCart>> UpdateCart(UserCart cart)
        {
            var neworupdatedcart = await cartItemRepo.UpdateCartAsync(cart);
            if (neworupdatedcart is null)
                return BadRequest("there is error occuerd when add or update cart");
            return neworupdatedcart;
        }
    }
}
