using Microsoft.EntityFrameworkCore;
using MyBook.API.Helpers;
using MyBook.API.ResourceParameters;
using MyBook.DbContexts;
using MyBook.Entities;

namespace MyBook.Services
{
    public class MyBookRepository : IMyBookRepository
    {
        private readonly MyBookDbContext _dbContext;

        public MyBookRepository(MyBookDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Book?> GetBookAsync(Guid id)
        {
            return await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<PagedList<Book>> GetBooksAsync(BooksResourceParameters booksResourceParameters)
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

            if (booksResourceParameters.SearchQuery != null)
            {
                collection = collection.Where(b =>
                    (b.Title + b.Description + b.Publisher + b.ISBN + b.Category + b.Author.Name + b.Author.Description)
                    .Contains(booksResourceParameters.SearchQuery));
            }

            return await PagedList<Book>.CreateAsync(collection,
                booksResourceParameters.PageNumber, booksResourceParameters.PageSize);
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

        public async Task<bool> AuthorExistsAsync(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _dbContext.Authors.AnyAsync(a => a.Id == authorId);
        }

        public void UpdateBook(Book book)
        {
            // not needed for this implementation
        }

        public void DeleteBook(Book book)
        {
            _dbContext.Books.Remove(book);
        }

        public async Task<bool> SaveAsync()
        {
            return (await _dbContext.SaveChangesAsync() > 0);
        }
    }
}
