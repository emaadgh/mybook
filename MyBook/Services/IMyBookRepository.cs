using MyBook.API.ResourceParameters;
using MyBook.Entities;

namespace MyBook.Services
{
    public interface IMyBookRepository
    {
        Task<IEnumerable<Book?>> GetBooksAsync(BooksResourceParameters booksResourceParameters);
        Task<Book?> GetBookAsync(Guid id);
        Task<bool> AuthorExistsAsync(Guid authorId);
        void AddBook(Guid authorId, Book book);
        void UpdateBook(Book book);
        void DeleteBook(Book book);
        Task<bool> SaveAsync();
    }
}
