using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBook.Entities;

public class Book
{
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string? ISBN { get; set; }
    [MaxLength(1500)]
    public string? Description { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset PublicationDate { get; set; }
    public string? Publisher { get; set; }

    [ForeignKey("AuthorId")]
    public Author Author { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public Book(string title)
    {
        Title = title;
    }
}
