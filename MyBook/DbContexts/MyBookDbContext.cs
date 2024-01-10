using Microsoft.EntityFrameworkCore;
using MyBook.Entities;

namespace MyBook.DbContexts
{
    public class MyBookDbContext : DbContext
    {
        public MyBookDbContext(DbContextOptions<MyBookDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;

    }
}
