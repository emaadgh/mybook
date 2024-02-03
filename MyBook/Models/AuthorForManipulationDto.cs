using System.ComponentModel.DataAnnotations;

namespace MyBook.API.Models
{
    public abstract class AuthorForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(100, ErrorMessage = "The name shouldn't have more than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public string? Description { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
