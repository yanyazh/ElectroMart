using System.ComponentModel.DataAnnotations;

namespace myStore.Models
{
    public class News
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? ImagePath { get; set; }  // Optional image path for the article
    }
}

