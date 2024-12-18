using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myStore.Models;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


public class ProductsController : Controller
{
    private readonly ECommerceContext _context;

    public ProductsController(ECommerceContext context)
    {
        _context = context;
    }

    public IActionResult Index(string searchString, int? categoryId)
    {
        var products = _context.Products.Include(p => p.Category).AsQueryable();

        // Apply search filter if search string is provided
        if (!string.IsNullOrEmpty(searchString))
        {
            products = products.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
        }

        // Apply category filter if categoryId is provided
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId);
        }

        // Get the categories with product count
        var categories = _context.Categories
            .Select(c => new CategoryWithProductCount
            {
                Category = c,
                ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
            })
            .ToList();

        // Get recent news articles
        var recentNews = _context.NewsArticles
            .Take(5)
            .OrderByDescending(n => n.PublishedDate)
            .ToList();

        // Create the view model and pass data to the view
        var viewModel = new ProductPageViewModel
        {
            Products = products.ToList(),
            Categories = categories,
            RecentNews = recentNews
        };

        return View(viewModel); // Ensure you're passing the ProductPageViewModel
    }





    // GET: Products/Create
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Ensure the images directory exists
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save the image to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream); // Automatically disposes of stream
                }

                product.ImagePath = "/images/" + fileName;
            }

            _context.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Categories = _context.Categories.ToList();  // Repopulate categories if validation fails
        return View(product);
    }

    // GET: Products/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = _context.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            DeleteImageIfExists(product.ImagePath);  // Delete image if it exists
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();  // Async save
        }
        return RedirectToAction(nameof(Index));
    }


    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
                                     .Include(p => p.Category)
                                     .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(product);
    }

    private void EnsureImagesDirectoryExists()
    {
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private void DeleteImageIfExists(string? imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }


    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (imageFile != null && imageFile.Length > 0)
            {
                DeleteImageIfExists(existingProduct?.ImagePath);  // Delete old image if a new one is uploaded

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                EnsureImagesDirectoryExists();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImagePath = "/images/" + fileName;
            }
            else
            {
                product.ImagePath = existingProduct?.ImagePath;
            }

            _context.Update(product);
            await _context.SaveChangesAsync();  // Async save
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();  // Async load categories
        return View(product);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
                                     .Include(p => p.Category) // Include related category info
                                     .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }


}
