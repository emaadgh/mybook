﻿namespace MyBook.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}
