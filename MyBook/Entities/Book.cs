using System.ComponentModel.DataAnnotations;

namespace MyBook.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? ISBN { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
        public string? Publisher { get; set; }

        public Book(string title)
        {
            Title = title;
        }
    }
}
