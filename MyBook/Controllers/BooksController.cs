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

            return Ok(_mapper.Map<BookDto>(book).ShapeData(fields));
        }

        [Route("all")]
        [HttpGet]
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

            return Ok(_mapper.Map<IEnumerable<BookDto>>(books).ShapeData(booksResourceParameters.Fields));
        }

        [HttpPost]
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

            var bookToReturn = _mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBook", new { bookToReturn.Id }, bookToReturn);
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
    }
}
