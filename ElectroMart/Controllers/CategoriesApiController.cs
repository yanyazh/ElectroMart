using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace myStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ECommerceContext _context;

        public CategoriesApiController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ImagePath // Including only the properties needed
                })
                .ToListAsync();

            return Ok(categories);  // Returns a 200 OK response with the list of categories
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.ImagePath // Including only the properties needed
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(); // Returns 404 if the category is not found
            }

            return Ok(category); // Returns the category in a 200 OK response
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null"); // Returns 400 Bad Request if the category is null
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category); // Returns 201 Created
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id)
            {
                return BadRequest("Category ID mismatch"); // Returns 400 if the IDs don't match
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(); // Returns 404 if the category doesn't exist
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Returns 204 No Content when the update is successful
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(); // Returns 404 if the category doesn't exist
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent(); // Returns 204 No Content when the delete is successful
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id); // Checks if category exists in the database
        }
    }
}
