using FluentAssertions;
using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Models;

namespace MyBook.Test.Services;

public class PropertyMappingServiceTests
{
    private readonly PropertyMappingService _propertyMappingService;

    public PropertyMappingServiceTests()
    {
        _propertyMappingService = new PropertyMappingService();
    }

    [Fact]
    public void GetPropertyMapping_ValidMappingExists_MustReturnsMapping()
    {
        // Act
        var mapping = _propertyMappingService.GetPropertyMapping<BookDto, Book>();

        // Assert
        mapping.Should().NotBeNull();
        mapping.Should().NotBeEmpty();
    }

    [Fact]
    public void GetPropertyMapping_ValidMappingDoesNotExist_MustThrowsException()
    {
        // Act & Assert
        Action action = () => _propertyMappingService.GetPropertyMapping<BookDto, Author>();
        action.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData("Id,Title,AuthorId")]
    [InlineData("Id")]
    [InlineData("")]
    public void ValidMappingExistsFor_ValidOrderBy_MustReturnsTrue(string orderBy)
    {
        // Act
        var result = _propertyMappingService.ValidMappingExistsFor<BookDto, Book>(orderBy);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("InvalidProperty")]
    [InlineData("Id,InvalidProperty")]
    public void ValidMappingExistsFor_InvalidOrderBy_MustReturnsFalse(string orderBy)
    {
        // Act
        var result = _propertyMappingService.ValidMappingExistsFor<BookDto, Book>(orderBy);

        // Assert
        result.Should().BeFalse();
    }
}
