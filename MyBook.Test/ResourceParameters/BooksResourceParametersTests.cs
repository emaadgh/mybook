using MyBook.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBook.Test.ResourceParameters
{
    public class BooksResourceParametersTests
    {
        [Fact]
        public void PageSize_SetToValueGreaterThanMaxPageSize_MustEqualsMaxPageSize()
        {
            // Arrange
            var parameters = new BooksResourceParameters();

            // Act
            parameters.PageSize = 20;

            // Assert
            Assert.Equal(parameters.MaxPageSize, parameters.PageSize);
        }

        [Fact]
        public void PageSize_SetToValidValue_MustNotChangeValue()
        {
            // Arrange
            var parameters = new BooksResourceParameters();

            // Act
            parameters.PageSize = 7;

            // Assert
            Assert.Equal(7, parameters.PageSize);
        }
    }
}
