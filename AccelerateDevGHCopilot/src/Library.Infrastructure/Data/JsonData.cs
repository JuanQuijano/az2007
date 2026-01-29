using System.Text.Json;
using Library.ApplicationCore.Entities;
using Microsoft.Extensions.Configuration;

namespace Library.Infrastructure.Data;

public class JsonData
{
    public List<Author>? Authors { get; set; }
    public List<Book>? Books { get; set; }
    public List<BookItem>? BookItems { get; set; }
    public List<Patron>? Patrons { get; set; }
    public List<Loan>? Loans { get; set; }

    private readonly string _authorsPath;
    private readonly string _booksPath;
    private readonly string _bookItemsPath;
    private readonly string _patronsPath;
    private readonly string _loansPath;
    private readonly SemaphoreSlim _loadLock = new(1, 1);

    public JsonData(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var section = configuration.GetSection("JsonPaths");
        _authorsPath = section["Authors"] ?? Path.Combine("Json", "Authors.json");
        _booksPath = section["Books"] ?? Path.Combine("Json", "Books.json");
        _bookItemsPath = section["BookItems"] ?? Path.Combine("Json", "BookItems.json");
        _patronsPath = section["Patrons"] ?? Path.Combine("Json", "Patrons.json");
        _loansPath = section["Loans"] ?? Path.Combine("Json", "Loans.json");
    }

    public async Task EnsureDataLoaded()
    {
        if (Patrons != null) return;
        
        await _loadLock.WaitAsync();
        try
        {
            if (Patrons == null)
            {
                await LoadData();
            }
        }
        finally
        {
            _loadLock.Release();
        }
    }

    public async Task LoadData()
    {
        Authors = await LoadJson<List<Author>>(_authorsPath);
        Books = await LoadJson<List<Book>>(_booksPath);
        BookItems = await LoadJson<List<BookItem>>(_bookItemsPath);
        Patrons = await LoadJson<List<Patron>>(_patronsPath);
        Loans = await LoadJson<List<Loan>>(_loansPath);
    }

    public async Task SaveLoans(IEnumerable<Loan> loans)
    {
        List<Loan> loanList = new List<Loan>();
        foreach (var l in loans)
        {
            Loan loan = new Loan
            {
                // making sure only a subset of properties is set and saved
                Id = l.Id,
                BookItemId = l.BookItemId,
                PatronId = l.PatronId,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate
            };
            loanList.Add(loan);
        }
        await SaveJson(_loansPath, loanList);
    }

    public async Task SavePatrons(IEnumerable<Patron> patrons)
    {
        await SaveJson(_patronsPath, patrons.Select(p => new Patron
        {
            Id = p.Id,
            Name = p.Name,
            MembershipStart = p.MembershipStart,
            MembershipEnd = p.MembershipEnd,
            ImageName = p.ImageName,
        }).ToList());
    }

    private async Task SaveJson<T>(string filePath, T data)
    {
        using (FileStream jsonStream = File.Create(filePath))
        {
            await JsonSerializer.SerializeAsync(jsonStream, data);
        }
    }

    public List<Patron> GetPopulatedPatrons(IEnumerable<Patron> patrons)
    {
        return patrons.Select(GetPopulatedPatron).ToList();
    }

    public Patron GetPopulatedPatron(Patron p)
    {
        Patron populated = new Patron
        {
            Id = p.Id,
            Name = p.Name,
            ImageName = p.ImageName,
            MembershipStart = p.MembershipStart,
            MembershipEnd = p.MembershipEnd,
            Loans = Loans!
                .Where(loan => loan.PatronId == p.Id)
                .Select(GetPopulatedLoan)
                .ToList()
        };

        return populated;
    }

    public Loan GetPopulatedLoan(Loan l)
    {
        Loan populated = new Loan
        {
            Id = l.Id,
            BookItemId = l.BookItemId,
            PatronId = l.PatronId,
            LoanDate = l.LoanDate,
            DueDate = l.DueDate,
            ReturnDate = l.ReturnDate
        };

        var bookItem = BookItems!.FirstOrDefault(bi => bi.Id == l.BookItemId);
        if (bookItem != null)
        {
            populated.BookItem = GetPopulatedBookItem(bookItem);
        }

        populated.Patron = Patrons!.FirstOrDefault(p => p.Id == l.PatronId);

        return populated;
    }

    public BookItem GetPopulatedBookItem(BookItem bi)
    {
        BookItem populated = new BookItem
        {
            Id = bi.Id,
            BookId = bi.BookId,
            AcquisitionDate = bi.AcquisitionDate,
            Condition = bi.Condition
        };

        var book = Books!.FirstOrDefault(b => b.Id == bi.BookId);
        if (book != null)
        {
            populated.Book = GetPopulatedBook(book);
        }

        return populated;
    }

    public Book GetPopulatedBook(Book b)
    {
        Book populated = new Book
        {
            Id = b.Id,
            Title = b.Title,
            AuthorId = b.AuthorId,
            Genre = b.Genre,
            ISBN = b.ISBN,
            ImageName = b.ImageName
        };

        var author = Authors!.FirstOrDefault(a => a.Id == b.AuthorId);
        if (author != null)
        {
            populated.Author = new Author
            {
                Id = author.Id,
                Name = author.Name
            };
        }

        return populated;
    }

    private async Task<T?> LoadJson<T>(string filePath)
    {
        using (FileStream jsonStream = File.OpenRead(filePath))
        {
            return await JsonSerializer.DeserializeAsync<T>(jsonStream);
        }
    }

}
