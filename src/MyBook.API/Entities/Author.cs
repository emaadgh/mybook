using System.ComponentModel.DataAnnotations;

namespace MyBook.Entities;

public class Author
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset DateOfBirth { get; set; }

    public Author(string name)
    {
        this.Name = name;
    }
}
