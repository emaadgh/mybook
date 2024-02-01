using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Controllers;
using MyBook.Entities;
using MyBook.Models;
using MyBook.Services;

namespace MyBook.Test
{
    public class BooksControllerTests
    {
        private readonly Mock<IMyBookRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockRepository = new Mock<IMyBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _propertyMappingService = new PropertyMappingService();
            _controller = new BooksController(_mockRepository.Object, _mockMapper.Object, _propertyMappingService);
        }

        [Fact]
        public async Task GetBook_BookExists_MustReturnsOkResult()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            var book = new Book("Book1");
            book.Id = bookId;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(new BookDto { Id = bookId });

            // Act
            var result = await _controller.GetBook(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<BookDto>(okResult.Value);
            Assert.Equal(bookId, model.Id);
        }

        [Fact]
        public async Task GetBook_BookNotExist_MustReturnsNotFound()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            Book? book = null;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBooks_InvalidOrderByParameter_MustReturnsBadRequest()
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
            var result = await _controller.GetBooks(booksResourceParameters);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetBooks_ValidOrderByParameter_MustReturnsOkObjectResult()
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
            var result = await _controller.GetBooks(booksResourceParameters);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<IEnumerable<BookDto>>(okObjectResult.Value);
        }
    }
}
