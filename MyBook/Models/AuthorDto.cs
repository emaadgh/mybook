using System.ComponentModel.DataAnnotations;

namespace MyBook.API.Models
{
    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
