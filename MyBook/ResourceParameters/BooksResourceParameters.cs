namespace MyBook.API.ResourceParameters
{
    public class BooksResourceParameters
    {
        const int maxPageSize = 10;

        public string? PublisherName { get; set; }

        public Guid? AuthorId { get; set; }

        public string? SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "Title";
    }
}
