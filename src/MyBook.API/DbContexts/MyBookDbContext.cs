using Microsoft.EntityFrameworkCore;
using MyBook.Entities;

namespace MyBook.DbContexts;

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
                Id = Guid.Parse("ad3d0a3d-006d-4a2d-817b-114cf7e22904"),
                DateOfBirth = new DateTime(1947, 9, 21)
            },
            new Author("Douglas Adams")
            {
                Id = Guid.Parse("d3b05403-79b9-460a-9d5e-a3641fd5a1b2"),
                DateOfBirth = new DateTime(1952, 3, 11)
            });

        modelBuilder.Entity<Book>().HasData(
            new Book("Harry Potter and the Half-Blood Prince (Harry Potter  #6)")
            {
                Id = Guid.Parse("ffba8a54-c990-4862-931e-927b35b3b003"),
                ISBN = "0747581088",
                PublicationDate = new DateTime(2006, 9, 16),
                Publisher = "Scholastic Inc.",
                AuthorId = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4")
            },
            new Book("Harry Potter and the Order of the Phoenix (Harry Potter  #5)")
            {
                Id = Guid.Parse("9a747b83-82d3-4968-a34f-dab4b5b9ee2b"),
                ISBN = "0747551006",
                PublicationDate = new DateTime(2004, 9, 1),
                Publisher = "Scholastic Inc.",
                AuthorId = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4")
            },
            new Book("Harry Potter and the Chamber of Secrets (Harry Potter  #2)")
            {
                Id = Guid.Parse("d7d654b5-97e9-4b91-a3c3-580ce3fdc73d"),
                ISBN = "0747538492",
                PublicationDate = new DateTime(2003, 1, 11),
                Publisher = "Scholastic Inc.",
                AuthorId = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4")
            },
            new Book("Harry Potter and the Prisoner of Azkaban (Harry Potter  #3)")
            {
                Id = Guid.Parse("4599eef6-d41f-45a7-8c75-c9c9172ea62b"),
                ISBN = "0747542155",
                PublicationDate = new DateTime(2004, 1, 5),
                Publisher = "Scholastic Inc.",
                AuthorId = Guid.Parse("a4848a8c-49ed-45ec-9ef8-ea3761793db4")
            },
            new Book("The Hitchhiker's Guide to the Galaxy (Hitchhiker's Guide to the Galaxy  #1)")
            {
                Id = Guid.Parse("9e1d17e0-9fbd-4242-888a-ea225218ffb3"),
                ISBN = "0330258648",
                PublicationDate = new DateTime(2004, 1, 5),
                Publisher = "Scholastic Inc.",
                AuthorId = Guid.Parse("d3b05403-79b9-460a-9d5e-a3641fd5a1b2")
            });

        base.OnModelCreating(modelBuilder);
    }
}
