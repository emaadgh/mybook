using AutoMapper;
using MyBook.API.Models;
using MyBook.Entities;
using MyBook.Models;

namespace MyBook.API.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<BookForCreationDto, Book>();
            CreateMap<BookForUpdateDto, Book>();
        }
    }
}
