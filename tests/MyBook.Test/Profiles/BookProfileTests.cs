using AutoMapper;
using FluentAssertions;
using MyBook.API.Models;
using MyBook.API.Profiles;
using MyBook.Entities;
using MyBook.Models;

namespace MyBook.Test.Profiles;

public class BookProfileTests
{
    private readonly IMapper _mapper;
    public BookProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BookProfile>();
        });
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_BookToBookDto_MustPropertyValuesBeEqual()
    {
        // Arrange
        var book = new Book("Test Book Title");

        // Act
        var bookDto = _mapper.Map<BookDto>(book);

        // Assert
        bookDto.Id.Should().Be(book.Id);
        bookDto.Title.Should().Be(book.Title);
    }

    [Fact]
    public void Map_BookToBook_ReverseMappingIsValid()
    {
        // Arrange
        var book = new Book("Test Book Title");

        // Act
        var bookMapped = _mapper.Map<Book>(_mapper.Map<BookDto>(book));

        // Assert
        bookMapped.Title.Should().Be(book.Title);
    }

    [Fact]
    public void Map_BookForCreationDtoToBook_MustPropertyValuesBeEqual()
    {
        // Arrange
        var bookForCreationDto = new BookForCreationDto { Title = "Test Book Title" };

        // Act
        var book = _mapper.Map<Book>(bookForCreationDto);

        // Assert
        book.Title.Should().Be(bookForCreationDto.Title);
    }

    [Fact]
    public void Map_BookForUpdateDtoToBook_MustPropertyValuesBeEqual()
    {
        // Arrange
        var bookForUpdateDto = new BookForUpdateDto { Title = "Test Updated Title" };

        // Act
        var book = _mapper.Map<Book>(bookForUpdateDto);

        // Assert
        book.Title.Should().Be(bookForUpdateDto.Title);
    }

    [Fact]
    public void Map_BookForUpdateDtoToBookForUpdateDto_ReverseMappingIsValid()
    {
        // Arrange
        var bookForUpdateDto = new BookForUpdateDto { Title = "Test Updated Title" };

        // Act
        var bookForUpdateDtoMapped = _mapper.Map<BookForUpdateDto>(_mapper.Map<Book>(bookForUpdateDto));

        // Assert
        bookForUpdateDtoMapped.Title.Should().Be(bookForUpdateDto.Title);
    }
}
