using System.ComponentModel.DataAnnotations;

namespace MyBook.API.Models
{
    public class BookForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Title { get; set; } = string.Empty;

        public string? ISBN { get; set; }

        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public string? Description { get; set; }

        [MaxLength(50, ErrorMessage = "The category shouldn't have more than 50 characters.")]
        public string? Category { get; set; }

        public DateTimeOffset PublicationDate { get; set; }

        [MaxLength(50, ErrorMessage = "The publisher shouldn't have more than 50 characters.")]
        public string? Publisher { get; set; }
    }
}
