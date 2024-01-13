using MyBook.Entities;

namespace MyBook.Services
{
    public interface IMyBookRepository
    {
        Task<IEnumerable<Book?>> GetBooksAsync(Guid authorId);
        Task<Book?> GetBookAsync(Guid id);
        Task<bool> AuthorExistsAsync(Guid authorId);
    }
}
