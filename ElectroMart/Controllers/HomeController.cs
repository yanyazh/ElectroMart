using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using myStore.Models;
using System.Diagnostics;
using System.Linq;

namespace myStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ECommerceContext _context;

        public HomeController(ILogger<HomeController> logger, ECommerceContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchString, int? categoryId)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply search filter if search string is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            // Apply category filter if categoryId is provided
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            // Get the filtered products (limit to 10 for now)
            var products = productsQuery.Take(10).ToList();

            // Create a new view model for products
            var viewModel = new ProductPageViewModel
            {
                Products = products
            };

            // If it's an AJAX request, return only the product cards HTML
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FeaturedProducts", viewModel.Products);
            }

            // If it's a normal request, return the full view with all data
            var categoriesWithCounts = _context.Categories
                .Select(c => new CategoryWithProductCount
                {
                    Category = c,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToList();

            var recentNews = _context.NewsArticles
                .OrderByDescending(n => n.PublishedDate)
                .Take(3)
                .ToList();

            viewModel.Categories = categoriesWithCounts;
            viewModel.RecentNews = recentNews;

            return View(viewModel);
        }







        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
