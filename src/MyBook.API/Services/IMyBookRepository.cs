using MyBook.API.Helpers;
using MyBook.API.ResourceParameters;
using MyBook.Entities;

namespace MyBook.Services;

public interface IMyBookRepository
{
    Task<PagedList<Book>> GetBooksAsync(BooksResourceParameters booksResourceParameters, CancellationToken cancellationToken = default);
    Task<Book?> GetBookAsync(Guid id, CancellationToken cancellationToken = default);
    void AddBook(Guid authorId, Book book);
    void UpdateBook(Book book);
    void DeleteBook(Book book);
    Task<bool> AuthorExistsAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<bool> BookForAuthorExistsAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task<PagedList<Author>> GetAuthorsAsync(AuthorsResourceParameters authorsResourceParameters, CancellationToken cancellationToken = default);
    Task<Author?> GetAuthorAsync(Guid id, CancellationToken cancellationToken = default);
    void AddAuthor(Author author);
    void UpdateAuthor(Author? author);
    void DeleteAuthor(Author author);
    Task<bool> SaveAsync(CancellationToken cancellationToken = default);
}
