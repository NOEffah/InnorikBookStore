using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public required string Author { get; set; }
        [Required]
        public string? Genre { get; set; }
        public string? ImageUrl { get; set; } // Stores path or filename

    }
}
