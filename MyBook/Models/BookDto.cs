namespace MyBook.Models
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? ISBN { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTimeOffset PublicationDate { get; set; }
        public string? Publisher { get; set; }
    }
}
