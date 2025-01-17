using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace myStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsApiController : ControllerBase
    {
        private readonly ECommerceContext _context;

        public NewsApiController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: api/news
        [HttpGet]
        public async Task<IActionResult> GetNewsArticles()
        {
            var newsArticles = await _context.NewsArticles
                .OrderByDescending(n => n.PublishedDate)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.ImagePath,
                    n.PublishedDate
                })
                .ToListAsync();

            return Ok(newsArticles);
        }

        // GET: api/news/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsArticle(int id)
        {
            var newsArticle = await _context.NewsArticles
                .Where(n => n.Id == id)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Content,
                    n.ImagePath,
                    n.PublishedDate
                })
                .FirstOrDefaultAsync();

            if (newsArticle == null)
            {
                return NotFound();
            }

            return Ok(newsArticle);
        }

        // POST: api/news
        [HttpPost]
        public async Task<IActionResult> CreateNewsArticle([FromForm] News news, IFormFile? imageFile)
        {
            if (news == null)
            {
                return BadRequest("News article cannot be null.");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/newsImages", fileName);

                if (!Directory.Exists("wwwroot/newsImages"))
                {
                    Directory.CreateDirectory("wwwroot/newsImages");
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                news.ImagePath = "/newsImages/" + fileName;
            }

            news.PublishedDate = DateTime.Now;
            _context.NewsArticles.Add(news);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNewsArticle), new { id = news.Id }, news);
        }

        // PUT: api/news/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNewsArticle(int id, [FromForm] News news, IFormFile? imageFile)
        {
            if (id != news.Id)
            {
                return BadRequest("News ID mismatch.");
            }

            var existingNews = await _context.NewsArticles.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
            if (existingNews == null)
            {
                return NotFound();
            }

            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(existingNews.ImagePath))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingNews.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/newsImages", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                news.ImagePath = "/newsImages/" + fileName;
            }
            else
            {
                news.ImagePath = existingNews.ImagePath;
            }

            if (news.PublishedDate == DateTime.MinValue)
            {
                news.PublishedDate = existingNews.PublishedDate;
            }

            _context.Entry(news).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/news/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsArticle(int id)
        {
            var news = await _context.NewsArticles.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            // Delete associated image
            if (!string.IsNullOrEmpty(news.ImagePath))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", news.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.NewsArticles.Remove(news);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NewsArticleExists(int id)
        {
            return _context.NewsArticles.Any(e => e.Id == id);
        }
    }
}
