using System.ComponentModel.DataAnnotations;

namespace myStore.Models
{
    public class ProductPageViewModel
    {
        public List<CategoryWithProductCount> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<News>? RecentNews { get; set; }
    }

    public class CategoryWithProductCount
    {
        public Category Category { get; set; }
        public int ProductCount { get; set; }
    }
}


