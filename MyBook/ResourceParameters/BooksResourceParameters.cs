namespace MyBook.API.ResourceParameters
{
    public class BooksResourceParameters
    {
        public string? PublisherName { get; set; }

        public Guid? AuthorId { get; set; }

        public string? SearchQuery { get; set; }
    }
}
