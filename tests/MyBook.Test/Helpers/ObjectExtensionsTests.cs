using FluentAssertions;
using MyBook.API.Helpers;

namespace MyBook.Test.Helpers;

public class ObjectExtensionsTests
{
    TestObject testObjectSource;
    public ObjectExtensionsTests()
    {
        testObjectSource = new TestObject { Id = 1, Name = "Test", Description = "Description" };
    }

    [Fact]
    public void ShapeData_NoFieldsSpecified_MustShapesAllProperties()
    {
        // Act
        var shapedData = testObjectSource.ShapeData(null, null);

        // Assert
        ((IDictionary<string, object?>)shapedData).Count.Should().Be(3);
    }

    [Fact]
    public void ShapeData_FieldsSpecified_MustShapesAccordingToFields()
    {
        // Act
        var shapedData = testObjectSource.ShapeData("Id,Name", null);

        // Assert
        IDictionary<string, object?> shapedDataDictionary = shapedData;

        shapedDataDictionary.Count.Should().Be(2);
        shapedDataDictionary.Keys.Should().Contain("Id");
        shapedDataDictionary.Keys.Should().Contain("Name");
    }

    [Fact]
    public void ShapeData_RequiredFieldsProvided_MustIncludesRequiredFields()
    {
        // Act
        var shapedData = testObjectSource.ShapeData("Id", "Name");

        // Assert
        IDictionary<string, object?> shapedDataDictionary = shapedData;

        shapedDataDictionary.Count.Should().Be(2);
        shapedDataDictionary.Keys.Should().Contain("Id");
        shapedDataDictionary.Keys.Should().Contain("Name");
    }

    [Fact]
    public void ShapeData_FieldDoesNotExist_MustThrowsException()
    {
        // Act & Assert
        Action action = () => testObjectSource.ShapeData("InvalidField", null);
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void ShapeData_SourceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        TestObject? nullTestObject = null;

        // Act & Assert
        Action action = () => nullTestObject.ShapeData("Id", null);
        action.Should().Throw<ArgumentNullException>();
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
