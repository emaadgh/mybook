using AutoMapper;
using FluentAssertions;
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

namespace MyBook.Test.Controllers;

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

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Accept = "application/json";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };
    }

    [Fact]
    public async Task GetBookWithoutLinks_BookExists_MustReturnsOkResult()
    {
        // Arrange
        var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
        var book = new Book("Book1");
        book.Id = bookId;

        CancellationToken cancellationToken = CancellationToken.None;

        _mockRepository.Setup(repo => repo.GetBookAsync(bookId, cancellationToken)).ReturnsAsync(book);
        _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(new BookDto { Id = bookId });

        // Act
        var result = await _controller.GetBook(bookId, null, cancellationToken);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var model = okResult.Value.Should().BeAssignableTo<ExpandoObject>().Subject;

        var expandoDictionary = model as IDictionary<string, object?>;
        expandoDictionary["Id"].Should().Be(bookId);
    }

    [Fact]
    public async Task GetBookWithoutLinks_FieldIsInvalid_MustReturnsBadRequest()
    {
        // Arrange
        var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
        string fields = "InvalidFields";

        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.GetBook(bookId, fields, cancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetBookWithoutLinks_BookNotExist_MustReturnsNotFound()
    {
        // Arrange
        var bookId = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003");
        Book? book = null;

        CancellationToken cancellationToken = CancellationToken.None;

        _mockRepository.Setup(repo => repo.GetBookAsync(bookId, cancellationToken)).ReturnsAsync(book);

        // Act
        var result = await _controller.GetBook(bookId, null, cancellationToken);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetBooksWithoutLinks_InvalidOrderByParameter_MustReturnsBadRequest()
    {
        // Arrange
        BooksResourceParameters booksResourceParameters = new BooksResourceParameters();
        booksResourceParameters.OrderBy = "NotAValidParameter";

        CancellationToken cancellationToken = CancellationToken.None;

        _mockRepository.Setup(repo => repo.GetBooksAsync(booksResourceParameters, cancellationToken)).ReturnsAsync(new API.Helpers.PagedList<Book>(new List<Book>(), 0, 0, 4));

        // Act
        var result = await _controller.GetBooks(booksResourceParameters, cancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetBooksWithoutLinks_ValidOrderByParameter_MustReturnsOkObjectResult()
    {
        // Arrange
        BooksResourceParameters booksResourceParameters = new BooksResourceParameters();
        booksResourceParameters.OrderBy = "Title";

        CancellationToken cancellationToken = CancellationToken.None;

        _mockRepository.Setup(repo => repo.GetBooksAsync(booksResourceParameters, cancellationToken)).ReturnsAsync(new API.Helpers.PagedList<Book>(new List<Book>(), 0, 0, 4));

        // Act
        var result = await _controller.GetBooks(booksResourceParameters, cancellationToken);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task CreateBookForAuthorWithoutLinks_AuthorNotExist_MustReturnsNotFound()
    {
        // Arrange
        Guid authorId = Guid.Empty;

        BookForCreationDto book = new BookForCreationDto();

        CancellationToken cancellationToken = CancellationToken.None;

        // Act
        var result = await _controller.CreateBookForAuthor(authorId, book, cancellationToken);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
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

        CancellationToken cancellationToken = CancellationToken.None;

        _mockRepository.Setup(repo => repo.AuthorExistsAsync(authorId, cancellationToken)).ReturnsAsync(true);

        _mockMapper.Setup(mapper => mapper.Map<Book>(bookForCreation)).Returns(book);
        _mockMapper.Setup(mapper => mapper.Map<BookDto>(book)).Returns(bookDto);

        // Act
        var result = await _controller.CreateBookForAuthor(authorId, bookForCreation, cancellationToken);

        // Assert
        var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
        bookDto.Id.Should().Be(Guid.Parse($"{createdAtRouteResult.RouteValues?["id"]}"));
    }
}
