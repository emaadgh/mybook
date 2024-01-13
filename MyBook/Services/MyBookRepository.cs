using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> AuthorExistsAsync(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _dbContext.Authors.AnyAsync(a => a.Id == authorId);
        }

        public async Task<Book?> GetBookAsync(Guid id)
        {
            return await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Book?>> GetBooksAsync(Guid authorId)
        {
            return await _dbContext.Books.Where(b => b.AuthorId == authorId).ToListAsync();
        }
    }
}
