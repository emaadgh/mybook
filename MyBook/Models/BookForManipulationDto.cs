using System.ComponentModel.DataAnnotations;

namespace MyBook.API.Models
{
    /// <summary>
    /// Data transfer object (DTO) for manipulating book entities.
    /// </summary>
    public class BookForManipulationDto
    {
        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ISBN (International Standard Book Number) of the book.
        /// </summary>
        public string? ISBN { get; set; }

        /// <summary>
        /// Gets or sets the description of the book.
        /// </summary>
        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the book.
        /// </summary>
        [MaxLength(50, ErrorMessage = "The category shouldn't have more than 50 characters.")]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the publication date of the book.
        /// </summary>
        public DateTimeOffset PublicationDate { get; set; }

        /// <summary>
        /// Gets or sets the publisher of the book.
        /// </summary>
        [MaxLength(50, ErrorMessage = "The publisher shouldn't have more than 50 characters.")]
        public string? Publisher { get; set; }
    }
}
