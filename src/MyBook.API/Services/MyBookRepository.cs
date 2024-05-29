using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBook.API.Helpers;
using MyBook.API.Models;
using MyBook.API.ResourceParameters;
using MyBook.API.Services;
using MyBook.DbContexts;
using MyBook.Entities;
using MyBook.Models;

namespace MyBook.Services
{
    public class MyBookRepository : IMyBookRepository
    {
        private readonly MyBookDbContext _dbContext;
        private readonly IPropertyMappingService _propertyMappingService;

        public MyBookRepository(MyBookDbContext dbContext, IPropertyMappingService propertyMappingService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public async Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedList<Book>> GetBooksAsync(BooksResourceParameters booksResourceParameters, CancellationToken cancellationToken = default)
        {
            if (booksResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(booksResourceParameters));
            }

            var collection = _dbContext.Books as IQueryable<Book>;

            if (!string.IsNullOrEmpty(booksResourceParameters.PublisherName))
            {
                var publisherName = booksResourceParameters.PublisherName.Trim();
                collection = collection.Where(b => b.Publisher == publisherName);
            }

            if (booksResourceParameters.AuthorId != null)
            {
                collection = collection.Where(b => b.AuthorId == booksResourceParameters.AuthorId);
            }

            if (!string.IsNullOrEmpty(booksResourceParameters.SearchQuery))
            {
                collection = collection.Where(b =>
                    (b.Title + b.Description + b.Publisher + b.ISBN + b.Category + b.Author.Name + b.Author.Description)
                    .Contains(booksResourceParameters.SearchQuery));
            }

            if (!string.IsNullOrEmpty(booksResourceParameters.OrderBy))
            {
                var bookProperyMappingDictionary = _propertyMappingService.GetPropertyMapping<BookDto, Book>();

                collection = collection.ApplySort(booksResourceParameters.OrderBy,
                bookProperyMappingDictionary);
            }

            return await PagedList<Book>.CreateAsync(collection,
                booksResourceParameters.PageNumber, booksResourceParameters.PageSize, cancellationToken);
        }

        public void AddBook(Guid authorId, Book book)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (book == null)
            {
                throw new ArgumentNullException(nameof(book));
            }

            book.AuthorId = authorId;
            _dbContext.Books.Add(book);
        }

        public async Task<bool> AuthorExistsAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _dbContext.Authors.AnyAsync(a => a.Id == authorId, cancellationToken);
        }

        public async Task<bool> BookForAuthorExistsAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _dbContext.Books.AnyAsync(b => b.AuthorId == authorId, cancellationToken);
        }

        public void UpdateBook(Book book)
        {
            // not needed for this implementation
        }

        public void DeleteBook(Book book)
        {
            _dbContext.Books.Remove(book);
        }

        public async Task<PagedList<Author>> GetAuthorsAsync(AuthorsResourceParameters authorsResourceParameters, CancellationToken cancellationToken = default)
        {
            if (authorsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            var collection = _dbContext.Authors as IQueryable<Author>;

            if (!string.IsNullOrEmpty(authorsResourceParameters.FullName))
            {
                var name = authorsResourceParameters.FullName.Trim();
                collection = collection.Where(a => a.Name == name);
            }

            if (authorsResourceParameters.DateOfBirth != DateTimeOffset.MinValue)
            {
                collection = collection.Where(b => b.DateOfBirth == authorsResourceParameters.DateOfBirth);
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.SearchQuery))
            {
                collection = collection.Where(a =>
                    (a.Name + a.Description + a.DateOfBirth)
                    .Contains(authorsResourceParameters.SearchQuery));
            }

            if (!string.IsNullOrEmpty(authorsResourceParameters.OrderBy))
            {
                var authorProperyMappingDictionary = _propertyMappingService.GetPropertyMapping<AuthorDto, Author>();

                collection = collection.ApplySort(authorsResourceParameters.OrderBy, authorProperyMappingDictionary);
            }

            return await PagedList<Author>.CreateAsync(collection,
                authorsResourceParameters.PageNumber, authorsResourceParameters.PageSize, cancellationToken);
        }

        public async Task<Author?> GetAuthorAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Authors.Where(a => a.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _dbContext.Authors.Add(author);
        }

        public void UpdateAuthor(Author? author)
        {
            // not needed for this implementation
        }

        public void DeleteAuthor(Author author)
        {
            _dbContext.Authors.Remove(author);
        }
        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return (await _dbContext.SaveChangesAsync(cancellationToken) > 0);
        }
    }
}
