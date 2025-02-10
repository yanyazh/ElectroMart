using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace myStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Ensures that only authenticated users can access the cart API
    public class CartApiController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartApiController(ECommerceContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class AddToCartRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == request.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += request.Quantity;
            }
            else
            {
                var cart = new Cart
                {
                    UserId = user.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };

                _context.Carts.Add(cart);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added to cart." });
        }


        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            //var USERID = "001be9bb-292c-49cb-8741-e4edcfcd926b";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                //.Where(c => c.UserId == USERID)
                .Select(c => new
                {
                    c.Id,
                    c.ProductId,
                    ProductName = c.Product.Name,
                    c.Quantity,
                    c.Product.Price,
                    TotalPrice = c.Quantity * c.Product.Price
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        [HttpDelete("RemoveFromCart/{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product removed from cart successfully" });
        }
        public class RemoveFromCartRequest
        {
            public int ProductId { get; set; }
        }


        [HttpDelete("RemoveFromCartByProductId")]
        public async Task<IActionResult> RemoveFromCartByProductId([FromBody] RemoveFromCartRequest request)
        {
            // Get the current authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            // Find the cart item based on product ID and current user
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == request.ProductId && c.UserId == user.Id);

            if (cartItem == null)
            {
                return NotFound(new { message = "Product not found in the cart." });
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product removed from cart successfully." });
        }



    }
}

