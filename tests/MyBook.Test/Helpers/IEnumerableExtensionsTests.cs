using FluentAssertions;
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
        ((IDictionary<string, object?>)shapedData.ToList().First()).Count.Should().Be(3);
    }

    [Fact]
    public void ShapeData_FieldsSpecified_MustShapesAccordingToFields()
    {
        // Act
        var shapedData = testObjectSources.ShapeData("Id,Name", null);

        // Assert
        var firstItemDictionary = (IDictionary<string, object?>)shapedData.ToList().First();
        firstItemDictionary.Count.Should().Be(2);
        firstItemDictionary.Keys.Contains("Id");
        firstItemDictionary.Keys.Contains("Name");
    }

    [Fact]
    public void ShapeData_RequiredFieldsProvided_MustIncludesRequiredFields()
    {
        // Act
        var shapedData = testObjectSources.ShapeData("Id", "Name");

        // Assert
        var firstItemDictionary = (IDictionary<string, object?>)shapedData.ToList().First();

        firstItemDictionary.Count.Should().Be(2);
        firstItemDictionary.Keys.Contains("Id");
        firstItemDictionary.Keys.Contains("Name");
    }

    [Fact]
    public void ShapeData_FieldDoesNotExist_MustThrowsException()
    {
        // Act & Assert
        Action action = () => testObjectSources.ShapeData("InvalidField", null);
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void ShapeData_SourceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        List<TestObject>? nullTestObject = null;

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
