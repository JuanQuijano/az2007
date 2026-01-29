using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface IBookRepository
{
    Task<Book?> GetBookByTitle(string title);
    Task<List<BookItem>> GetBookItemsByBookId(int bookId);
}
