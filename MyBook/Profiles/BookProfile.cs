using AutoMapper;
using MyBook.Entities;
using MyBook.Models;

namespace MyBook.API.Profiles
{
    public class BookProfile :Profile
    {
        public BookProfile() 
        {
            CreateMap<Book, BookDto>();
        }
    }
}
