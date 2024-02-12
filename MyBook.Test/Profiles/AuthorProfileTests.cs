using AutoMapper;
using MyBook.API.Models;
using MyBook.API.Profiles;
using MyBook.Entities;

namespace MyBook.Test.Profiles
{
    public class AuthorProfileTests
    {
        private readonly IMapper _mapper;
        public AuthorProfileTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AuthorProfile>();
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void Map_AuthorToAuthorDto_MustPropertyValuesBeEqual()
        {
            // Arrange
            var author = new Author("Test Author Name");

            // Act
            var authorDto = _mapper.Map<AuthorDto>(author);

            // Assert
            Assert.Equal(author.Id, authorDto.Id);
            Assert.Equal(author.Name, authorDto.Name);
        }

        [Fact]
        public void Map_AuthorForCreationDtoToAuthor_MustPropertyValuesBeEqual()
        {
            // Arrange
            var authorForCreationDto = new AuthorForCreationDto { Name = "Test Author Name" };

            // Act
            var author = _mapper.Map<Author>(authorForCreationDto);

            // Assert
            Assert.Equal(authorForCreationDto.Name, author.Name);
        }

        [Fact]
        public void Map_AuthorForUpdateDtoToAuthor_MustPropertyValuesBeEqual()
        {
            // Arrange
            var authorForUpdateDto = new AuthorForUpdateDto { Name = "Test Updated Name" };

            // Act
            var author = _mapper.Map<Author>(authorForUpdateDto);

            // Assert
            Assert.Equal(authorForUpdateDto.Name, author.Name);
        }

        [Fact]
        public void Map_AuthorForUpdateDtoToAuthorForUpdateDto_ReverseMappingIsValid()
        {
            // Arrange
            var authorForUpdateDto = new AuthorForUpdateDto { Name = "Test Updated Name" };

            // Act
            var authorForUpdateDtoMapped = _mapper.Map<AuthorForUpdateDto>(_mapper.Map<Author>(authorForUpdateDto));

            // Assert
            Assert.Equal(authorForUpdateDto.Name, authorForUpdateDtoMapped.Name);
        }
    }
}
