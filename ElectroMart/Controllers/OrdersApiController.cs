using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace myStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderApiController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderApiController(ECommerceContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class CreateOrderRequest
        {
            public string ShippingAddress { get; set; }
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            // Get the authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            // Retrieve all cart items for the current user
            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest(new { message = "Your cart is empty. Cannot create an order." });
            }

            // Create a new order and add the cart items as order items
            var order = new Orders
            {
                UserId = user.Id,
                ShippingAddress = request.ShippingAddress,
                OrderDate = DateTime.UtcNow,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Clear the user's cart after creating the order
            _context.Carts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order created successfully.", OrderId = order.Id });
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.ShippingAddress,
                    Items = o.OrderItems.Select(oi => new
                    {
                        oi.ProductId,
                        oi.Product.Name,
                        oi.Quantity,
                        oi.UnitPrice,
                        TotalPrice = oi.Quantity * oi.UnitPrice
                    }),
                    OrderTotal = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
