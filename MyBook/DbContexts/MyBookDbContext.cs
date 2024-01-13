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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed the database with dummy data
            modelBuilder.Entity<Author>().HasData(
                new Author("J.K. Rowling")
                {
                    Id = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4"),
                    DateOfBirth = new DateTime(1965, 7, 31)
                },
                new Author("Stephen King")
                {
                    Id = Guid.NewGuid(),
                    DateOfBirth = new DateTime(1947, 9, 21)
                });

            modelBuilder.Entity<Book>().HasData(
                new Book("Harry Potter and the Half-Blood Prince (Harry Potter  #6)")
                {
                    Id = Guid.NewGuid(),
                    ISBN = "439785960",
                    PublicationDate = new DateTime(2006, 9, 16),
                    Publisher = "Scholastic Inc.",
                    AuthorId = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4")
                },
                new Book("Harry Potter and the Order of the Phoenix (Harry Potter  #5)")
                {
                    Id = Guid.NewGuid(),
                    ISBN = "439358078",
                    PublicationDate = new DateTime(2004, 9, 1),
                    Publisher = "Scholastic Inc.",
                    AuthorId = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4")
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
