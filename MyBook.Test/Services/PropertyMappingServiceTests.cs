using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBook.Test.Services
{
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
            Assert.NotNull(mapping);
            Assert.NotEmpty(mapping);
        }

        [Fact]
        public void GetPropertyMapping_ValidMappingDoesNotExist_MustThrowsException()
        {
            // Act & Assert
            Assert.Throws<Exception>(() => _propertyMappingService.GetPropertyMapping<BookDto, Author>());
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
            Assert.True(result);
        }

        [Theory]
        [InlineData("InvalidProperty")]
        [InlineData("Id,InvalidProperty")]
        public void ValidMappingExistsFor_InvalidOrderBy_MustReturnsFalse(string orderBy)
        {
            // Act
            var result = _propertyMappingService.ValidMappingExistsFor<BookDto, Book>(orderBy);

            // Assert
            Assert.False(result);
        }
    }
}
