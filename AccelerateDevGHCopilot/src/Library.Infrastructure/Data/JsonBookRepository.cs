using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.Infrastructure.Data;

public class JsonBookRepository : IBookRepository
{
    private readonly JsonData _jsonData;

    public JsonBookRepository(JsonData jsonData)
    {
        _jsonData = jsonData;
    }

    public async Task<Book?> GetBookByTitle(string title)
    {
        await _jsonData.EnsureDataLoaded();

        foreach (Book book in _jsonData.Books!)
        {
            if (book.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                return _jsonData.GetPopulatedBook(book);
            }
        }
        return null;
    }

    public async Task<List<BookItem>> GetBookItemsByBookId(int bookId)
    {
        await _jsonData.EnsureDataLoaded();

        List<BookItem> bookItems = new List<BookItem>();
        foreach (BookItem bookItem in _jsonData.BookItems!)
        {
            if (bookItem.BookId == bookId)
            {
                bookItems.Add(_jsonData.GetPopulatedBookItem(bookItem));
            }
        }
        return bookItems;
    }
}
