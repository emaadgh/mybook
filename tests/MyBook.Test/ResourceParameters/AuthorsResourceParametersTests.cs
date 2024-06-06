using FluentAssertions;
using MyBook.API.ResourceParameters;

namespace MyBook.Test.ResourceParameters;

public class AuthorsResourceParametersTests
{
    [Fact]
    public void PageSize_SetToValueGreaterThanMaxPageSize_MustEqualsMaxPageSize()
    {
        // Arrange
        var parameters = new AuthorsResourceParameters();

        // Act
        parameters.PageSize = 20;

        // Assert
        parameters.PageSize.Should().Be(parameters.MaxPageSize);
    }

    [Fact]
    public void PageSize_SetToValidValue_MustNotChangeValue()
    {
        // Arrange
        var parameters = new AuthorsResourceParameters();

        // Act
        parameters.PageSize = 7;

        // Assert
        parameters.PageSize.Should().Be(7);
    }
}
