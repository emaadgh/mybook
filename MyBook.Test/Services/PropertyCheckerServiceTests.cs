﻿using MyBook.API.Services;
using MyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBook.Test.Services
{
    public class PropertyCheckerServiceTests
    {
        private readonly PropertyCheckerService _propertyCheckerService;

        public PropertyCheckerServiceTests()
        {
            _propertyCheckerService = new PropertyCheckerService();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void TypeHasProperties_NoFields_MustReturnsTrue(string? fields)
        {
            // Act
            var result = _propertyCheckerService.TypeHasProperties<BookDto>(fields);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TypeHasProperties_ValidFields_MustReturnsTrue()
        {
            // Arrange
            var fields = "Id,Title,AuthorId";

            // Act
            var result = _propertyCheckerService.TypeHasProperties<BookDto>(fields);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TypeHasProperties_InvalidFields_MustReturnsFalse()
        {
            // Arrange
            var fields = "InvalidProperty";

            // Act
            var result = _propertyCheckerService.TypeHasProperties<BookDto>(fields);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TypeHasProperties_FieldsContainLeadingOrTrailingSpaces_MustReturnsTrue()
        {
            // Arrange
            var fields = " Id, Title,AuthorId ";

            // Act
            var result = _propertyCheckerService.TypeHasProperties<BookDto>(fields);

            // Assert
            Assert.True(result);
        }
    }
}
