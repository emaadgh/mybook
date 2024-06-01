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
        Assert.Equal(3, ((IDictionary<string, object?>)shapedData).Count);
    }

    [Fact]
    public void ShapeData_FieldsSpecified_MustShapesAccordingToFields()
    {
        // Act
        var shapedData = testObjectSource.ShapeData("Id,Name", null);

        // Assert
        Assert.Equal(2, ((IDictionary<string, object?>)shapedData).Count);
        Assert.Contains("Id", ((IDictionary<string, object?>)shapedData).Keys);
        Assert.Contains("Name", ((IDictionary<string, object?>)shapedData).Keys);
    }

    [Fact]
    public void ShapeData_RequiredFieldsProvided_MustIncludesRequiredFields()
    {
        // Act
        var shapedData = testObjectSource.ShapeData("Id", "Name");

        // Assert
        Assert.Equal(2, ((IDictionary<string, object?>)shapedData).Count);
        Assert.Contains("Id", ((IDictionary<string, object?>)shapedData).Keys);
        Assert.Contains("Name", ((IDictionary<string, object?>)shapedData).Keys);
    }

    [Fact]
    public void ShapeData_FieldDoesNotExist_MustThrowsException()
    {
        // Act & Assert
        Assert.Throws<Exception>(() => testObjectSource.ShapeData("InvalidField", null));
    }

    [Fact]
    public void ShapeData_SourceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        TestObject? nullTestObject = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullTestObject.ShapeData("Id", null));
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
