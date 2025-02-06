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

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            Console.WriteLine("alo blat");

            //var USERID = "001be9bb-292c-49cb-8741-e4edcfcd926b";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //return Unauthorized();
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ProductId == productId);
                //.FirstOrDefaultAsync(c => c.UserId == USERID && c.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var cart = new Cart
                {
                    UserId = user.Id,
                    //UserId = USERID,
                    ProductId = productId,
                    Quantity = quantity
                };

                _context.Carts.Add(cart);
            }


            await _context.SaveChangesAsync();

            return Ok("Product added");
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
    }
}
