using MyBook.API.Models;
using MyBook.Entities;
using MyBook.Models;

namespace MyBook.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _bookPropertyMapping =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new(new[] { "Id" }) },
            { "AuthorId", new(new[] { "AuthorId" }) },
            { "Title", new(new[] { "Title" }) },
            { "ISBN", new(new[] { "ISBN" }) },
            { "Description", new(new[] { "Description" }) },
            { "Category", new(new[] { "Category" }) },
            { "PublicationDate", new(new[] { "PublicationDate" }) },
            { "Publisher", new(new[] { "Publisher" }) },
        };

        private readonly Dictionary<string, PropertyMappingValue> _authorProperyMapping =
            new(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new (new[] {"Id"}) },
                {"Name", new (new[] {"Name"}) },
                {"Description", new (new[] {"Description"}) },
                {"DateOfBirth", new (new[] {"DateOfBirth"}) }
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<BookDto, Book>(_bookPropertyMapping));
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorProperyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _propertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First().MappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string orderBy)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = orderBy.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
