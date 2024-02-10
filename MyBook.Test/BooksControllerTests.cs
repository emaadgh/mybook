using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Controllers;
using MyBook.Entities;
using MyBook.Models;
using MyBook.Services;
using System.Dynamic;

namespace MyBook.Test
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
        public async Task GetBook_BookExists_MustReturnsOkResult()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            var book = new Book("Book1");
            book.Id = bookId;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(new BookDto { Id = bookId });

            // Mocking IUrlHelper
            var expectedProtocol = "testprotocol://";
            var expectedHost = "www.example.com";

            var httpContext = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = expectedProtocol,
                    Host = new HostString(expectedHost),
                }
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var mockUrlHelper = CreateMockUrlHelper(actionContext);
            mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>())).Returns("callbackUrl");
            mockUrlHelper.Setup(h => h.Action(It.IsAny<UrlActionContext>())).Returns("callbackUrl");
            _controller.Url = mockUrlHelper.Object;

            // Act
            var result = await _controller.GetBook(bookId, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<ExpandoObject>(okResult.Value);
            var expandoDictionary = model as IDictionary<string, object?>;
            Assert.Equal(bookId, expandoDictionary["Id"]);
        }

        [Fact]
        public async Task GetBook_BookNotExist_MustReturnsNotFound()
        {
            // Arrange
            var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
            Book? book = null;

            _mockRepository.Setup(repo => repo.GetBookAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(bookId, null);

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
            Assert.IsType<BadRequestObjectResult>(result);
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

            // Mocking IUrlHelper
            var expectedProtocol = "testprotocol://";
            var expectedHost = "www.example.com";

            var httpContext = new DefaultHttpContext
            {
                Request =
                {
                    Scheme = expectedProtocol,
                    Host = new HostString(expectedHost),
                }
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var mockUrlHelper = CreateMockUrlHelper(actionContext);
            mockUrlHelper.Setup(h => h.RouteUrl(It.IsAny<UrlRouteContext>())).Returns("callbackUrl");
            mockUrlHelper.Setup(h => h.Action(It.IsAny<UrlActionContext>())).Returns("callbackUrl");
            _controller.Url = mockUrlHelper.Object;

            // Act
            var result = await _controller.GetBooks(booksResourceParameters);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
        }
        private static Mock<IUrlHelper> CreateMockUrlHelper(ActionContext context = null)
        {
            if (context == null)
            {
                context = GetActionContextForPage("/Page");
            }

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.SetupGet(h => h.ActionContext)
                .Returns(context);
            return urlHelper;
        }

        private static ActionContext GetActionContextForPage(string page)
        {
            return new ActionContext
            {
                ActionDescriptor = new ActionDescriptor
                {
                    RouteValues = new Dictionary<string, string>
                    {
                        { "page", page },
                    },
                },
                RouteData = new RouteData
                {
                    Values =
                    {
                        [ "page" ] = page
                    },
                },
            };
        }
    }
}
