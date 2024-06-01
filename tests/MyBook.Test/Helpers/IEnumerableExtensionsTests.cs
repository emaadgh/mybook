using MyBook.API.Helpers;
using System.Dynamic;

namespace MyBook.Test.Helpers;

public class IEnumerableExtensionsTests
{
    IEnumerable<TestObject> testObjectSources;
    public IEnumerableExtensionsTests()
    {
        testObjectSources = new List<TestObject>()
        {
            new TestObject { Id = 1, Name = "Test1", Description = "Description1" },
            new TestObject { Id = 2, Name = "Test2", Description = "Description2" },
            new TestObject { Id = 2, Name = "Test3", Description = "Description3" },
            new TestObject { Id = 2, Name = "Test4", Description = "Description4" }
        };
    }

    [Fact]
    public void ShapeData_NoFieldsSpecified_MustShapesAllProperties()
    {
        // Act
        var shapedData = testObjectSources.ShapeData(null, null);

        // Assert
        Assert.Equal(3, ((IDictionary<string, object?>)((List<ExpandoObject>)shapedData).First()).Count);
    }

    [Fact]
    public void ShapeData_FieldsSpecified_MustShapesAccordingToFields()
    {
        // Act
        var shapedData = testObjectSources.ShapeData("Id,Name", null);

        // Assert
        var firstItemDictionary = ((IDictionary<string, object?>)((List<ExpandoObject>)shapedData).First());

        Assert.Equal(2, firstItemDictionary.Count);
        Assert.Contains("Id", firstItemDictionary.Keys);
        Assert.Contains("Name", firstItemDictionary.Keys);
    }

    [Fact]
    public void ShapeData_RequiredFieldsProvided_MustIncludesRequiredFields()
    {
        // Act
        var shapedData = testObjectSources.ShapeData("Id", "Name");

        // Assert
        var firstItemDictionary = ((IDictionary<string, object?>)((List<ExpandoObject>)shapedData).First());

        Assert.Equal(2, firstItemDictionary.Count);
        Assert.Contains("Id", firstItemDictionary.Keys);
        Assert.Contains("Name", firstItemDictionary.Keys);
    }

    [Fact]
    public void ShapeData_FieldDoesNotExist_MustThrowsException()
    {
        // Act & Assert
        Assert.Throws<Exception>(() => testObjectSources.ShapeData("InvalidField", null));
    }

    [Fact]
    public void ShapeData_SourceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        List<TestObject>? nullTestObject = null;

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
