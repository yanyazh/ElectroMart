using System.ComponentModel.DataAnnotations;

namespace myStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImagePath { get; set; }

        // Foreign Key for Category
        public int? CategoryId { get; set; }  // Nullable to handle products without a category
        public Category? Category { get; set; }
    }
}
