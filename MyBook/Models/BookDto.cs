﻿namespace MyBook.Models
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
    }
}
