using AutoMapper;
using MyBook.API.Models;
using MyBook.Entities;

namespace MyBook.API.Profiles;

public class AuthorProfile :Profile
{
    public AuthorProfile() 
    {
        CreateMap<Author, AuthorDto>();
        CreateMap<AuthorForCreationDto, Author>();
        CreateMap<AuthorForUpdateDto, Author>().ReverseMap();
    }
}
