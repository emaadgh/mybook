using System.ComponentModel.DataAnnotations;

namespace MyBook.API.Models
{
    /// <summary>
    /// Data transfer object (DTO) for manipulating author entities.
    /// </summary>
    public abstract class AuthorForManipulationDto
    {
        /// <summary>
        /// Gets or sets the name of the author.
        /// </summary>
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(100, ErrorMessage = "The name shouldn't have more than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the author.
        /// </summary>
        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the author.
        /// </summary>
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
