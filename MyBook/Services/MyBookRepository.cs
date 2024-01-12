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

        public async Task<Book?> GetBookAsync(int id)
        {
            return await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
        }
    }
}
