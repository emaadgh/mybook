using Microsoft.AspNetCore.Mvc;

namespace MyBook.API.ResourceParameters
{
    public class AuthorsResourceParameters
    {
        const int maxPageSize = 10;

        public string? FullName { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public string? SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 5;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "Name";

        public string? Fields { get; set; }
    }
}
