using MyBook.API.Helpers;
using MyBook.API.ResourceParameters;
using MyBook.Entities;

namespace MyBook.Services
{
    public interface IMyBookRepository
    {
        Task<PagedList<Book>> GetBooksAsync(BooksResourceParameters booksResourceParameters);
        Task<Book?> GetBookAsync(Guid id);
        void AddBook(Guid authorId, Book book);
        void UpdateBook(Book book);
        void DeleteBook(Book book);
        Task<bool> AuthorExistsAsync(Guid authorId);
        Task<bool> BookForAuthorExistsAsync(Guid authorId);
        Task<PagedList<Author>> GetAuthorsAsync(AuthorsResourceParameters authorsResourceParameters);
        Task<Author?> GetAuthorAsync(Guid id);
        void AddAuthor(Author author);
        void UpdateAuthor(Author? author);
        void DeleteAuthor(Author author);
        Task<bool> SaveAsync();
    }
}
