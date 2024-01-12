using MyBook.Entities;

namespace MyBook.Services
{
    public interface IMyBookRepository
    {
        Task<Book?> GetBookAsync(int id);
    }
}
