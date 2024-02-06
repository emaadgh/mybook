using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MyBook.API.Helpers;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.Entities;
using MyBook.Models;
using MyBook.Services;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MyBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMyBookRepository _myBookRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        private readonly string _bookShapingRequiredFields = JsonNamingPolicy.CamelCase.ConvertName(nameof(BookDto.Id)) + ',' + JsonNamingPolicy.CamelCase.ConvertName(nameof(BookDto.AuthorId));
        
        public BooksController(IMyBookRepository myBookRepository, IMapper mapper,
            IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService, ProblemDetailsFactory problemDetailsFactory)
        {
            _myBookRepository = myBookRepository ?? throw new ArgumentNullException(nameof(myBookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [HttpGet(Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetBook(Guid id, string? fields)
        {
            if (!_propertyCheckerService.TypeHasProperties<BookDto>(fields))
            {
                return BadRequest(_problemDetailsFactory.CreateProblemDetails(HttpContext,
                    statusCode: 400,
                    detail: $"Not all requested data shaping fields exist on " +
                    $"the resource: {fields}"));
            }

            var book = await _myBookRepository.GetBookAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            var links = CreateLinksForBook(id, book.AuthorId, fields);

            var shapedBooks = _mapper.Map<BookDto>(book).ShapeData(fields, _bookShapingRequiredFields) as IDictionary<string, object?>;
            shapedBooks.Add("links", links);

            return Ok(shapedBooks);
        }

        [HttpGet("all", Name = "GetBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetBooks([FromQuery] BooksResourceParameters booksResourceParameters)
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

            var books = await _myBookRepository.GetBooksAsync(booksResourceParameters);

            var paginationMetadata = new
            {
                totalCount = books.TotalCount,
                pageSize = books.PageSize,
                currentPage = books.CurrentPage,
                totalPages = books.TotalPages
            };

            Response.Headers.Append("X-Pagination",
                   JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForBooks(booksResourceParameters, books.HasNext, books.HasPrevious);

            var shapedBooks = _mapper.Map<IEnumerable<BookDto>>(books).ShapeData(booksResourceParameters.Fields, _bookShapingRequiredFields);

            var booksWithLinks = shapedBooks.Select(book =>
            {
                var bookDictionary = book as IDictionary<string, object?>;

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

        [HttpPost(Name = "CreateBookForAuthor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateBookForAuthor(Guid authorId, BookForCreationDto book)
        {
            if (!await _myBookRepository.AuthorExistsAsync(authorId))
            {
                return NotFound(book);
            }

            var bookEntity = _mapper.Map<Entities.Book>(book);

            _myBookRepository.AddBook(authorId, bookEntity);
            await _myBookRepository.SaveAsync();

            var links = CreateLinksForBook(bookEntity.Id, authorId, null);

            var bookDto = _mapper.Map<BookDto>(bookEntity);

            var shapedBookWithLinksToReturn = bookDto.ShapeData(null, null) as IDictionary<string, object?>;
            shapedBookWithLinksToReturn.Add("links", links);

            return CreatedAtRoute("GetBook", new { id = shapedBookWithLinksToReturn["Id"] }, shapedBookWithLinksToReturn);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateBook(Guid bookId, Guid authorId, BookForUpdateDto book)
        {
            if (!await _myBookRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _myBookRepository.GetBookAsync(bookId);

            if (bookFromRepo == null)
            {
                var bookToAdd = _mapper.Map<Book>(book);

                _myBookRepository.AddBook(authorId, bookToAdd);
                await _myBookRepository.SaveAsync();

                var bookToReturn = _mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBook", new { bookToReturn.Id }, bookToReturn);
            }

            _mapper.Map(book, bookFromRepo);

            _myBookRepository.UpdateBook(bookFromRepo);

            await _myBookRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PartiallyUpdateBook(Guid bookId, Guid authorId
            , JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            if (!await _myBookRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _myBookRepository.GetBookAsync(bookId);

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
                await _myBookRepository.SaveAsync();

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

            await _myBookRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBook(Guid bookId)
        {
            var bookFromRepo = await _myBookRepository.GetBookAsync(bookId);

            if (bookFromRepo == null)
            {
                return NotFound();
            }

            _myBookRepository.DeleteBook(bookFromRepo);
            await _myBookRepository.SaveAsync();

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
                  new(Url.ActionLink("GetAuthor", "Authors", new { id = authorId }),
                 "get_book_author",
                 "GET"));

            links.Add(
                  new(Url.Link("CreateBookForAuthor", new { authorId }),
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
}
