using System.ComponentModel.DataAnnotations;

namespace myStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }  // Category name

        public string? ImagePath { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
