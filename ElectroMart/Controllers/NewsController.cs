using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace myStore.Controllers
{
    public class NewsController : Controller
    {
        private readonly ECommerceContext _context;

        public NewsController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var newsArticles = await _context.NewsArticles.OrderByDescending(n => n.PublishedDate).ToListAsync();
            return View(newsArticles);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
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
                        imageFile.CopyTo(stream);
                    }

                    news.ImagePath = "/newsImages/" + fileName;
                }

                news.PublishedDate = DateTime.Now;
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.NewsArticles.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.NewsArticles.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, News news, IFormFile? imageFile)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingNews = await _context.NewsArticles.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
                if (existingNews == null)
                {
                    return NotFound();
                }

                // Preserve the existing image if no new image is uploaded
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(existingNews.ImagePath))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingNews.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Generate a unique filename and save the new image
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
                    // Retain the existing image path if no new image is provided
                    news.ImagePath = existingNews.ImagePath;
                }

                // Preserve the existing publish date if not modified
                if (news.PublishedDate == DateTime.MinValue)
                {
                    news.PublishedDate = existingNews.PublishedDate;
                }

                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NewsArticles.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.NewsArticles.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.NewsArticles.FindAsync(id);
            if (news != null)
            {
                _context.NewsArticles.Remove(news);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // Other CRUD methods (Edit, Delete, etc.) can be implemented similarly
    }
}
