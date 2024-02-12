using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using MyBook.API.Controllers;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Services;
using System.Dynamic;

namespace MyBook.Test.Controllers
{
    public class AuthorsControllerTests
    {
        private readonly Mock<IMyBookRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly Mock<ProblemDetailsFactory> _mockProblemDetailsFactory;

        private readonly AuthorsController _controller;

        public AuthorsControllerTests()
        {
            _mockRepository = new Mock<IMyBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _propertyMappingService = new PropertyMappingService();
            _propertyCheckerService = new PropertyCheckerService();
            _mockProblemDetailsFactory = new Mock<ProblemDetailsFactory>();

            _controller = new AuthorsController(_mockRepository.Object, _mockMapper.Object, _propertyMappingService, _propertyCheckerService, _mockProblemDetailsFactory.Object);
        }

        [Fact]
        public async Task GetAuthorWithoutLinks_AuthorExists_MustReturnsOkResult()
        {
            // Arrange
            var authorId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");

            Author author = new Author("test");

            AuthorDto authorDto = new AuthorDto();
            authorDto.Id = authorId;

            _mockRepository.Setup(repo => repo.GetAuthorAsync(authorId)).ReturnsAsync(author);
            _mockMapper.Setup(mapper => mapper.Map<AuthorDto>(author)).Returns(authorDto);

            // Act
            var result = await _controller.GetAuthorWithoutLinks(authorId, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ExpandoObject>(okResult.Value);
            var expandoDictionary = model as IDictionary<string, object?>;
            Assert.Equal(authorId, expandoDictionary["Id"]);
        }

        [Fact]
        public async Task GetAuthorWithoutLinks_FieldIsInvalid_MustReturnsBadRequest()
        {
            // Arrange
            var authorId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            string fields = "notValidField";

            // Act
            var result = await _controller.GetAuthorWithoutLinks(authorId, fields);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAuthorWithoutLinks_AuthorNotExist_MustReturnsNotFound()
        {
            // Arrange
            var authorId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            Author? author = null;

            _mockRepository.Setup(repo => repo.GetAuthorAsync(authorId)).ReturnsAsync(author);

            // Act
            var result = await _controller.GetAuthorWithoutLinks(authorId, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAuthorsWithoutLinks_InvalidOrderByParameter_MustReturnsBadRequest()
        {
            // Arrange
            AuthorsResourceParameters authorsResourceParameters = new AuthorsResourceParameters();
            authorsResourceParameters.OrderBy = "NotAValidParameter";

            var httpResponseHeadersMock = new Mock<IHeaderDictionary>();
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupGet(r => r.Headers).Returns(httpResponseHeadersMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _mockRepository.Setup(repo => repo.GetAuthorsAsync(authorsResourceParameters)).ReturnsAsync(new API.Helpers.PagedList<Author>(new List<Author>(), 0, 0, 4));

            // Act
            var result = await _controller.GetAuthorsWithoutLinks(authorsResourceParameters);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAuthorsWithoutLinks_ValidOrderByParameter_MustReturnsOkObjectResult()
        {
            // Arrange
            AuthorsResourceParameters authorsResourceParameters = new AuthorsResourceParameters();
            authorsResourceParameters.OrderBy = "Name";

            var httpResponseHeadersMock = new Mock<IHeaderDictionary>();
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupGet(r => r.Headers).Returns(httpResponseHeadersMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _mockRepository.Setup(repo => repo.GetAuthorsAsync(authorsResourceParameters)).ReturnsAsync(new API.Helpers.PagedList<Author>(new List<Author>(), 0, 0, 4));

            // Act
            var result = await _controller.GetAuthorsWithoutLinks(authorsResourceParameters);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateAuthorWithoutLinks_AuthorCreated_MustReturnsCreatedAtRouteResult()
        {
            // Arrange
            AuthorForCreationDto authorForCreationDto = new AuthorForCreationDto();
            authorForCreationDto.Name = "Name";

            Author author = new Author("test name");

            AuthorDto authorDto = new AuthorDto();
            authorDto.Id = Guid.NewGuid();

            _mockMapper.Setup(mapper => mapper.Map<Author>(authorForCreationDto)).Returns(author);
            _mockMapper.Setup(mapper => mapper.Map<AuthorDto>(author)).Returns(authorDto);

            // Act
            var result = await _controller.CreateAuthorWithoutLinks(authorForCreationDto);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(Guid.Parse(createdAtRouteResult.RouteValues["id"] + ""), authorDto.Id);
        }
    }
}
