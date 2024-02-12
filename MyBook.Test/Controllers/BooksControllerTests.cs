using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Controllers;
using MyBook.Entities;
using MyBook.Models;
using MyBook.Services;
using System.Dynamic;

namespace MyBook.Test.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IMyBookRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly Mock<ProblemDetailsFactory> _mockProblemDetailsFactory;

        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockRepository = new Mock<IMyBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _propertyMappingService = new PropertyMappingService();
            _propertyCheckerService = new PropertyCheckerService();
            _mockProblemDetailsFactory = new Mock<ProblemDetailsFactory>();

            _controller = new BooksController(_mockRepository.Object, _mockMapper.Object, _propertyMappingService, _propertyCheckerService, _mockProblemDetailsFactory.Object);
        }

        [Fact]
        public async Task GetBookWithoutLinks_BookExists_MustReturnsOkResult()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            var book = new Book("Book1");
            book.Id = bookId;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(new BookDto { Id = bookId });

            // Act
            var result = await _controller.GetBookWithoutLinks(bookId, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ExpandoObject>(okResult.Value);
            var expandoDictionary = model as IDictionary<string, object?>;
            Assert.Equal(bookId, expandoDictionary["Id"]);
        }

        [Fact]
        public async Task GetBookWithoutLinks_FieldIsInvalid_MustReturnsBadRequest()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            string fields = "InvalidFields";
            // Act
            var result = await _controller.GetBookWithoutLinks(bookId, fields);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetBookWithoutLinks_BookNotExist_MustReturnsNotFound()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            Book? book = null;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBookWithoutLinks(bookId, null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBooksWithoutLinks_InvalidOrderByParameter_MustReturnsBadRequest()
        {
            // Arrange
            BooksResourceParameters booksResourceParameters = new BooksResourceParameters();
            booksResourceParameters.OrderBy = "NotAValidParameter";

            var httpResponseHeadersMock = new Mock<IHeaderDictionary>();
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupGet(r => r.Headers).Returns(httpResponseHeadersMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _mockRepository.Setup(repo => repo.GetBooksAsync(booksResourceParameters)).ReturnsAsync(new API.Helpers.PagedList<Book>(new List<Book>(), 0, 0, 4));

            // Act
            var result = await _controller.GetBooksWithoutLinks(booksResourceParameters);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetBooksWithoutLinks_ValidOrderByParameter_MustReturnsOkObjectResult()
        {
            // Arrange
            BooksResourceParameters booksResourceParameters = new BooksResourceParameters();
            booksResourceParameters.OrderBy = "Title";

            var httpResponseHeadersMock = new Mock<IHeaderDictionary>();
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupGet(r => r.Headers).Returns(httpResponseHeadersMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(c => c.Response).Returns(httpResponseMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _mockRepository.Setup(repo => repo.GetBooksAsync(booksResourceParameters)).ReturnsAsync(new API.Helpers.PagedList<Book>(new List<Book>(), 0, 0, 4));

            // Act
            var result = await _controller.GetBooksWithoutLinks(booksResourceParameters);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateBookForAuthorWithoutLinks_AuthorNotExist_MustReturnsNotFound()
        {
            // Arrange
            Guid authorId = Guid.Empty;

            BookForCreationDto book = new BookForCreationDto();

            // Act
            var result = await _controller.CreateBookForAuthorWithoutLinks(authorId, book);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateBookForAuthorWithoutLinks_AuthorDoExist_MustReturnsCreatedAtRouteResult()
        {
            // Arrange
            Guid authorId = Guid.Parse("f3b858bb-5c40-4982-aca7-08dc2a4fce60");

            BookForCreationDto bookForCreation = new BookForCreationDto();
            Book book = new Book("test title");
            BookDto bookDto = new BookDto();
            bookDto.Id = Guid.NewGuid();

            _mockRepository.Setup(repo => repo.AuthorExistsAsync(authorId)).ReturnsAsync(true);

            _mockMapper.Setup(mapper => mapper.Map<Book>(bookForCreation)).Returns(book);
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(bookDto);

            // Act
            var result = await _controller.CreateBookForAuthorWithoutLinks(authorId, bookForCreation);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(Guid.Parse(createdAtRouteResult.RouteValues["id"] + ""), bookDto.Id);
        }
    }
}
