using MyBook.API.Helpers;
using MyBook.API.Services;

namespace MyBook.Test.Helpers
{
    public class IQueryableExtensionsTests
    {
        [Fact]
        public void ApplySort_ValidOrderByAndMappingDictionary_MustAppliesSorting()
        {
            // Arrange
            var source = new List<TestEntity>
            {
                new TestEntity { Id = 3, Name = "C" },
                new TestEntity { Id = 1, Name = "A" },
                new TestEntity { Id = 2, Name = "B" }
            }.AsQueryable();

            var mappingDictionary = new Dictionary<string, PropertyMappingValue>
            {
                { "Id", new PropertyMappingValue(new[] { "Id" }) },
                { "Name", new PropertyMappingValue(new[] { "Name" }) }
            };

            // Act
            var sortedQuery = source.ApplySort("Name desc", mappingDictionary).ToList();

            // Assert
            Assert.Equal("C", sortedQuery[0].Name);
            Assert.Equal("B", sortedQuery[1].Name);
            Assert.Equal("A", sortedQuery[2].Name);
        }

        [Fact]
        public void ApplySort_OrderByNullOrEmpty_MustReturnsOriginalQueryable()
        {
            // Arrange
            var source = new List<TestEntity>().AsQueryable();
            var mappingDictionary = new Dictionary<string, PropertyMappingValue>();

            // Act
            var sortedQuery = source.ApplySort("", mappingDictionary);

            // Assert
            Assert.Same(source, sortedQuery);
        }

        [Fact]
        public void ApplySort_NullSource_MustThrowsArgumentNullException()
        {
            // Arrange
            IQueryable<TestEntity>? source = null;
            var mappingDictionary = new Dictionary<string, PropertyMappingValue>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => source.ApplySort("Name", mappingDictionary));
        }

        [Fact]
        public void ApplySort_NullMappingDictionary_MustThrowsArgumentNullException()
        {
            // Arrange
            var source = new List<TestEntity>().AsQueryable();
            Dictionary<string, PropertyMappingValue>? mappingDictionary = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => source.ApplySort("Name", mappingDictionary));
        }

        [Fact]
        public void ApplySort_PropertyNotInMappingDictionary_MustThrowsArgumentException()
        {
            // Arrange
            var source = new List<TestEntity>().AsQueryable();
            var mappingDictionary = new Dictionary<string, PropertyMappingValue>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => source.ApplySort("InvalidProperty", mappingDictionary));
        }

        [Fact]
        public void ApplySort_RevertedSorting_MustRevertsSortOrder()
        {
            // Arrange
            var source = new List<TestEntity>
            {
                new TestEntity { Id = 3, Name = "C" },
                new TestEntity { Id = 1, Name = "A" },
                new TestEntity { Id = 2, Name = "B" }
            }.AsQueryable();

            var mappingDictionary = new Dictionary<string, PropertyMappingValue>
            {
                { "Id", new PropertyMappingValue(new[] { "Id" }, revert: true) },
            };

            // Act
            var sortedQuery = source.ApplySort("Id", mappingDictionary).ToList();

            // Assert
            Assert.Equal(3, sortedQuery[0].Id);
            Assert.Equal(2, sortedQuery[1].Id);
            Assert.Equal(1, sortedQuery[2].Id);
        }

        private class TestEntity
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
