using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Net.Http.Headers;
using MyBook.API.Helpers;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Models;
using MyBook.Services;
using System.Text.Json;

namespace MyBook.Controllers;

[Route("api/[controller]")]
[EnableRateLimiting("GlobalRateLimit")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IMyBookRepository _myBookRepository;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IPropertyCheckerService _propertyCheckerService;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    private readonly string _bookShapingRequiredFields = JsonNamingPolicy.CamelCase.ConvertName(nameof(BookDto.Id)) + ',' + JsonNamingPolicy.CamelCase.ConvertName(nameof(BookDto.AuthorId));

    private readonly string[] ValidHeadersWithoutLink = new[]
    {
        "*/*",
        "application/json",
        "application/vnd.mybook.book+json"
    };

    private readonly string[] ValidHeadersWithLink = new[]
    {
        "application/vnd.mybook.book.hateoas+json"
    };

    /// <summary>
    /// Constructor for BooksController.
    /// </summary>
    public BooksController(IMyBookRepository myBookRepository, IMapper mapper,
        IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService, ProblemDetailsFactory problemDetailsFactory)
    {
        _myBookRepository = myBookRepository ?? throw new ArgumentNullException(nameof(myBookRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _propertyCheckerService = propertyCheckerService;
        _problemDetailsFactory = problemDetailsFactory;
    }

    /// <summary>
    /// Get a book by id.
    /// </summary>
    /// <param name="id">The id of the book to get.</param>
    /// <param name="fields">The fields that are needed.</param>
    /// <param name="cancellationToken"> Cancellation token</param>
    /// <returns>An ActionResult containing the requested book.</returns>
    [HttpGet(Name = "GetBook")]
    [Produces("application/json", "application/vnd.mybook.book+json", "application/vnd.mybook.book.hateoas+json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> GetBook(Guid id, string? fields, CancellationToken cancellationToken)
    {
        var acceptHeader = Request.Headers.Accept.ToString();

        if (MediaTypeHeaderValue.TryParse(acceptHeader, out var parsedAccept))
        {
            if (ValidHeadersWithoutLink.Contains(parsedAccept.MediaType.Value))
            {
                return await GetBookInternal(id, fields, includeLinks: false, cancellationToken);
            }
            else if (ValidHeadersWithLink.Contains(parsedAccept.MediaType.Value))
            {
                return await GetBookInternal(id, fields, includeLinks: true, cancellationToken);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = "The Accept header value is not supported. Supported values are: "
                     + string.Join(", ", ValidHeadersWithoutLink) + string.Join(", ", ValidHeadersWithLink),
                    Status = StatusCodes.Status406NotAcceptable
                };

                return BadRequest(problemDetails);
            }
        }
        else
        {
            return BadRequest("Invalid Accept header format");
        }
    }

    private async Task<ActionResult> GetBookInternal(Guid id, string? fields, bool includeLinks, CancellationToken cancellationToken)
    {
        if (!_propertyCheckerService.TypeHasProperties<BookDto>(fields))
        {
            return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Not all requested data shaping fields exist on " +
                $"the resource: {fields}"));
        }

        var book = await _myBookRepository.GetBookAsync(id, cancellationToken);

        if (book == null)
        {
            return NotFound();
        }

        if (includeLinks)
        {
            var links = CreateLinksForBook(id, book.AuthorId, fields);

            var shapedBooks = _mapper.Map<BookDto>(book).ShapeData(fields, _bookShapingRequiredFields) as IDictionary<string, object?>;
            shapedBooks.Add("links", links);

            return Ok(shapedBooks);
        }
        else
        {
            return Ok(_mapper.Map<BookDto>(book).ShapeData(fields, _bookShapingRequiredFields));
        }
    }

    /// <summary>
    /// Get all books.
    /// </summary>
    /// <param name="booksResourceParameters">Parameters for filtering, searching, and pagination.</param>
    /// <param name="cancellationToken"> Cancellation token</param>
    /// <returns>An ActionResult containing a list of books.</returns>
    [HttpGet("all", Name = "GetBooks")]
    [Produces("application/json", "application/vnd.mybook.book+json", "application/vnd.mybook.book.hateoas+json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult> GetBooks([FromQuery] BooksResourceParameters booksResourceParameters, CancellationToken cancellationToken)
    {
        var acceptHeader = Request.Headers.Accept.ToString();

        if (MediaTypeHeaderValue.TryParse(acceptHeader, out var parsedAccept))
        {
            if (ValidHeadersWithoutLink.Contains(parsedAccept.MediaType.Value))
            {
                return await GetBooksInternal(booksResourceParameters, includeLinks: false, cancellationToken);
            }
            else if (ValidHeadersWithLink.Contains(parsedAccept.MediaType.Value))
            {
                return await GetBooksInternal(booksResourceParameters, includeLinks: true, cancellationToken);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = "The Accept header value is not supported. Supported values are: "
                     + string.Join(", ", ValidHeadersWithoutLink) + string.Join(", ", ValidHeadersWithLink),
                    Status = StatusCodes.Status406NotAcceptable
                };

                return BadRequest(problemDetails);
            }
        }
        else
        {
            return BadRequest("Invalid Accept header format");
        }
    }

    private async Task<ActionResult> GetBooksInternal(BooksResourceParameters booksResourceParameters, bool includeLinks, CancellationToken cancellationToken)
    {
        if (!_propertyMappingService
        .ValidMappingExistsFor<BookDto, Book>(
            booksResourceParameters.OrderBy))
        {
            return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Not all requested ordering fields exist on " +
                $"the resource: {booksResourceParameters.OrderBy}"));
        }

        if (!_propertyCheckerService.TypeHasProperties<BookDto>(booksResourceParameters.Fields))
        {
            return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Not all requested data shaping fields exist on " +
                $"the resource: {booksResourceParameters.Fields}"));
        }

        var books = await _myBookRepository.GetBooksAsync(booksResourceParameters, cancellationToken);

        var paginationMetadata = new
        {
            totalCount = books.TotalCount,
            pageSize = books.PageSize,
            currentPage = books.CurrentPage,
            totalPages = books.TotalPages
        };

        Response.Headers.Append("X-Pagination",
               JsonSerializer.Serialize(paginationMetadata));

        if (includeLinks)
        {
            var links = CreateLinksForBooks(booksResourceParameters, books.HasNext, books.HasPrevious);

            var shapedBooks = _mapper.Map<IEnumerable<BookDto>>(books).ShapeData(booksResourceParameters.Fields, _bookShapingRequiredFields);

            var booksWithLinks = shapedBooks.Select(book =>
            {
                var bookDictionary = book as IDictionary<string, object>;

                var bookLinks = CreateLinksForBook((Guid)bookDictionary["Id"], (Guid)bookDictionary["AuthorId"], null);

                bookDictionary.Add("links", bookLinks);

                return bookDictionary;
            });

            var linkedResourcesToReturn = new
            {
                value = booksWithLinks,
                links = links
            };

            return Ok(linkedResourcesToReturn);
        }
        else
        {
            return Ok(_mapper.Map<IEnumerable<BookDto>>(books).ShapeData(booksResourceParameters.Fields, _bookShapingRequiredFields));
        }
    }

    /// <summary>
    /// Create a new book for an author.
    /// </summary>
    /// <param name="authorId">The id of the author for whom the book is created.</param>
    /// <param name="book">The data for creating a new book.</param>
    /// <param name="cancellationToken"> Cancellation token</param>
    /// <returns>An ActionResult containing the created book.</returns>
    [HttpPost(Name = "CreateBookForAuthor")]
    [Produces("application/json", "application/vnd.mybook.book+json", "application/vnd.mybook.book.hateoas+json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateBookForAuthor(Guid authorId, BookForCreationDto book, CancellationToken cancellationToken)
    {
        var acceptHeader = Request.Headers.Accept.ToString();

        if (MediaTypeHeaderValue.TryParse(acceptHeader, out var parsedAccept))
        {
            if (ValidHeadersWithoutLink.Contains(parsedAccept.MediaType.Value))
            {
                return await CreateBookForAuthorInternal(authorId, book, includeLinks: false, cancellationToken);
            }
            else if (ValidHeadersWithLink.Contains(parsedAccept.MediaType.Value))
            {
                return await CreateBookForAuthorInternal(authorId, book, includeLinks: true, cancellationToken);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = "The Accept header value is not supported. Supported values are: "
                     + string.Join(", ", ValidHeadersWithoutLink) + string.Join(", ", ValidHeadersWithLink),
                    Status = StatusCodes.Status406NotAcceptable
                };

                return BadRequest(problemDetails);
            }
        }
        else
        {
            return BadRequest("Invalid Accept header format");
        }
    }

    private async Task<ActionResult> CreateBookForAuthorInternal(Guid authorId, BookForCreationDto book, bool includeLinks, CancellationToken cancellationToken)
    {
        if (!await _myBookRepository.AuthorExistsAsync(authorId, cancellationToken))
        {
            return NotFound(book);
        }

        var bookEntity = _mapper.Map<Book>(book);

        _myBookRepository.AddBook(authorId, bookEntity);
        await _myBookRepository.SaveAsync(cancellationToken);

        var bookDto = _mapper.Map<BookDto>(bookEntity);

        if (includeLinks)
        {
            var links = CreateLinksForBook(bookEntity.Id, authorId, null);

            var shapedBookWithLinksToReturn = bookDto.ShapeData(null, null) as IDictionary<string, object?>;
            shapedBookWithLinksToReturn.Add("links", links);

            return CreatedAtRoute("GetBook", new { id = shapedBookWithLinksToReturn["Id"] }, shapedBookWithLinksToReturn);
        }
        else
        {
            return CreatedAtRoute("GetBook", new { id = bookDto.Id }, bookDto);
        }
    }

    /// <summary>
    /// Update an existing book.
    /// </summary>
    /// <param name="bookId">The id of the book to update.</param>
    /// <param name="authorId">The id of the author of the book.</param>
    /// <param name="book">The data for updating the book.</param>
    /// <param name="cancellationToken"> Cancellation token</param>
    /// <returns>An ActionResult indicating success or failure of the operation.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateBook(Guid bookId, Guid authorId, BookForUpdateDto book, CancellationToken cancellationToken)
    {
        if (!await _myBookRepository.AuthorExistsAsync(authorId, cancellationToken))
        {
            return NotFound();
        }

        var bookFromRepo = await _myBookRepository.GetBookAsync(bookId, cancellationToken);

        if (bookFromRepo == null)
        {
            var bookToAdd = _mapper.Map<Book>(book);

            _myBookRepository.AddBook(authorId, bookToAdd);
            await _myBookRepository.SaveAsync(cancellationToken);

            var bookToReturn = _mapper.Map<BookDto>(bookToAdd);

            return CreatedAtRoute("GetBook", new { bookToReturn.Id }, bookToReturn);
        }

        _mapper.Map(book, bookFromRepo);

        _myBookRepository.UpdateBook(bookFromRepo);

        await _myBookRepository.SaveAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Partially update an existing book.
    /// </summary>
    /// <param name="bookId">The id of the book to update.</param>
    /// <param name="authorId">The id of the author of the book.</param>
    /// <param name="patchDocument">The patch document containing updates.</param>
    /// <param name="cancellationToken"> Cancellation token</param>
    /// <returns>An ActionResult indicating success or failure of the operation.</returns>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PartiallyUpdateBook(Guid bookId, Guid authorId
        , JsonPatchDocument<BookForUpdateDto> patchDocument, CancellationToken cancellationToken)
    {
        if (!await _myBookRepository.AuthorExistsAsync(authorId, cancellationToken))
        {
            return NotFound();
        }

        var bookFromRepo = await _myBookRepository.GetBookAsync(bookId, cancellationToken);

        if (bookFromRepo == null)
        {
            var bookDto = new BookForUpdateDto();

            patchDocument.ApplyTo(bookDto, ModelState);

            if (!TryValidateModel(bookDto))
            {
                return ValidationProblem(ModelState);
            }

            var bookToAdd = _mapper.Map<Book>(bookDto);
            bookToAdd.Id = bookId;

            _myBookRepository.AddBook(authorId, bookToAdd);
            await _myBookRepository.SaveAsync(cancellationToken);

            var bookToReturn = _mapper.Map<BookDto>(bookToAdd);

            return CreatedAtRoute("GetBook", new { bookId }, bookToReturn);
        }

        var bookToPatch = _mapper.Map<BookForUpdateDto>(bookFromRepo);
        patchDocument.ApplyTo(bookToPatch, ModelState);

        if (!TryValidateModel(bookToPatch))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(bookToPatch, bookFromRepo);

        _myBookRepository.UpdateBook(bookFromRepo);

        await _myBookRepository.SaveAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Delete a book.
    /// </summary>
    /// <param name="bookId">The id of the book to delete.</param>
    /// <param name="cancellationToken"> Cancellation token</param>
    /// <returns>An ActionResult indicating success or failure of the operation.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBook(Guid bookId, CancellationToken cancellationToken)
    {
        var bookFromRepo = await _myBookRepository.GetBookAsync(bookId, cancellationToken);

        if (bookFromRepo == null)
        {
            return NotFound();
        }

        _myBookRepository.DeleteBook(bookFromRepo);
        await _myBookRepository.SaveAsync(cancellationToken);

        return NoContent();
    }

    private List<LinkDto> CreateLinksForBook(Guid id, Guid authorId, string? fields)
    {
        var links = new List<LinkDto>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            links.Add(
              new(Url.Link("GetBook", new { id }),
              "self",
              "GET"));
        }
        else
        {
            links.Add(
              new(Url.Link("GetBook", new { id, fields }),
              "self",
              "GET"));
        }

        links.Add(
              new(Url.Link("GetAuthor", new { authorId }),
             "get_book_author",
             "GET"));

        links.Add(
              new(Url.Link("CreateBookForAuthor", new { AuthorId = authorId }),
              "create_book_for_author",
              "POST"));

        return links;
    }

    private IEnumerable<LinkDto> CreateLinksForBooks(BooksResourceParameters booksResourceParameters,
        bool hasNext, bool hasPrevious)
    {
        var links = new List<LinkDto>
        {
            // self 
            new(CreateBooksResourceUri(booksResourceParameters, ResourceUriType.Current),
                "self",
                "GET")
        };

        if (hasNext)
        {
            links.Add(
                new(CreateBooksResourceUri(booksResourceParameters, ResourceUriType.NextPage),
                    "nextPage",
                    "GET"));
        }

        if (hasPrevious)
        {
            links.Add(
                new(CreateBooksResourceUri(booksResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage",
                    "GET"));
        }

        return links;
    }

    private string? CreateBooksResourceUri(
        BooksResourceParameters booksResourceParameters,
        ResourceUriType type)
    {
        switch (type)
        {
            case ResourceUriType.PreviousPage:
                return Url.Link("GetBooks",
                    new
                    {
                        fields = booksResourceParameters.Fields,
                        orderBy = booksResourceParameters.OrderBy,
                        pageNumber = booksResourceParameters.PageNumber - 1,
                        pageSize = booksResourceParameters.PageSize,
                        searchQuery = booksResourceParameters.SearchQuery,
                        publisherName = booksResourceParameters.PublisherName,
                        authorId = booksResourceParameters.AuthorId
                    });
            case ResourceUriType.NextPage:
                return Url.Link("GetBooks",
                    new
                    {
                        fields = booksResourceParameters.Fields,
                        orderBy = booksResourceParameters.OrderBy,
                        pageNumber = booksResourceParameters.PageNumber + 1,
                        pageSize = booksResourceParameters.PageSize,
                        searchQuery = booksResourceParameters.SearchQuery,
                        publisherName = booksResourceParameters.PublisherName,
                        authorId = booksResourceParameters.AuthorId
                    });
            case ResourceUriType.Current:
            default:
                return Url.Link("GetBooks",
                    new
                    {
                        fields = booksResourceParameters.Fields,
                        orderBy = booksResourceParameters.OrderBy,
                        pageNumber = booksResourceParameters.PageNumber,
                        pageSize = booksResourceParameters.PageSize,
                        searchQuery = booksResourceParameters.SearchQuery,
                        publisherName = booksResourceParameters.PublisherName,
                        authorId = booksResourceParameters.AuthorId
                    });
        }
    }
}
