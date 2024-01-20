using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBook.API.Models;
using MyBook.Models;
using MyBook.Services;

namespace MyBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMyBookRepository _myBookRepository;
        private readonly IMapper _mapper;
        public BooksController(IMyBookRepository myBookRepository, IMapper mapper)
        {
            _myBookRepository = myBookRepository ?? throw new ArgumentNullException(nameof(myBookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetBook")]
        public async Task<ActionResult> GetBook(Guid id)
        {
            var book = await _myBookRepository.GetBookAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BookDto>(book));
        }

        [Route("all")]
        [HttpGet]
        public async Task<ActionResult> GetBooks(Guid authorId)
        {
            if (!await _myBookRepository.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }
            var booksForAuthor = await _myBookRepository.GetBooksAsync(authorId);

            return Ok(_mapper.Map<IEnumerable<BookDto>>(booksForAuthor));
        }

        [HttpPost]
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

    }
}
