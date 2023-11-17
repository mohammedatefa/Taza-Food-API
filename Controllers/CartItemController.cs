using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        [HttpGet("GetCart")]
        public async Task<ActionResult<UserCart>> GetCart(string id)
        {
            //var cart = await cartItemRepo.GetCartAsync(id);

            var cartId = User.FindFirstValue(ClaimTypes.Email.Split("@")[0]);

            if (string.IsNullOrEmpty(cartId))

                cartId = "DefualtUserId";

            var cart = await cartItemRepo.GetCartAsync(cartId);


            //if the cart is existed return it else create new one and return it 
            if (cart is null)
                return new UserCart(cartId);
            return cart;
        }

        [HttpPost("UpdateCart")]
        public async Task<ActionResult<UserCart>> UpdateCart(UserCart cart)
        {
            var neworupdatedcart = await cartItemRepo.UpdateCartAsync(cart);
            if (neworupdatedcart is null)
                return BadRequest("there is error occuerd when add or update cart");
            return neworupdatedcart;
        }

        [HttpDelete("DeleteCart")]
        public async Task<ActionResult<bool>> DeleteCart(string id)
        {
            return await cartItemRepo.DeleteCartAsync(id);
        }

        [HttpPost("AddToCart")]
        public async Task<ActionResult<UserCart>> AddToCart([FromBody] CartItem cartItem)
        {
            try
            {

                var userId = User.FindFirstValue(ClaimTypes.Email.Split("@")[0]);

                if (string.IsNullOrEmpty(userId))
                {
                    userId = "DefualtUserId";
                }

                var userCart = await cartItemRepo.GetCartAsync(userId) ?? new UserCart(userId);
                var existingItem = userCart.CartItems.FirstOrDefault(item => item.Id == cartItem.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += cartItem.Quantity;
                }
                else
                {
                    userCart.CartItems.Add(cartItem);
                }
                await cartItemRepo.UpdateCartAsync(userCart);

                return Ok(userCart);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding to cart: {ex.Message}");
            }
        }


        [HttpDelete("RemoveFromCart")]
        public async Task<ActionResult<UserCart>> RemoveFromCart(int productId)
        {
            try
            {
                var cartId = User.FindFirstValue(ClaimTypes.Email.Split("@")[0]);

                if (string.IsNullOrEmpty(cartId))

                    cartId = "DefualtUserId";

                var userCart = await cartItemRepo.GetCartAsync(cartId);


                if (userCart is null)
                {
                    return NotFound($"Cart not found for user with ID: {cartId}");
                }

                var existingItem = userCart.CartItems.FirstOrDefault(item => item.Id == productId);

                if (existingItem != null)
                {
                    userCart.CartItems.Remove(existingItem);
                    await cartItemRepo.UpdateCartAsync(userCart);
                    return Ok(userCart);
                }
                else
                {
                    return NotFound($"Product with ID {productId} not found in the cart");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error removing from cart: {ex.Message}");
            }
        }


    }
}
