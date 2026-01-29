using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.Infrastructure.Data;

public class JsonPatronRepository : IPatronRepository
{
    private readonly JsonData _jsonData;

    public JsonPatronRepository(JsonData jsonData)
    {
        _jsonData = jsonData ?? throw new ArgumentNullException(nameof(jsonData));
    }

    public async Task<List<Patron>> SearchPatrons(string searchInput)
    {
        ArgumentNullException.ThrowIfNull(searchInput);
        await _jsonData.EnsureDataLoaded();

        var searchResults = _jsonData.Patrons!
            .Where(patron => patron.Name.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            .OrderBy(patron => patron.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return _jsonData.GetPopulatedPatrons(searchResults);
    }

    public async Task<Patron?> GetPatron(int id)
    {
        await _jsonData.EnsureDataLoaded();

        var patron = _jsonData.Patrons!.FirstOrDefault(p => p.Id == id);
        return patron != null ? _jsonData.GetPopulatedPatron(patron) : null;
    }

    public async Task UpdatePatron(Patron patron)
    {
        ArgumentNullException.ThrowIfNull(patron);
        await _jsonData.EnsureDataLoaded();
        
        var existingPatron = _jsonData.Patrons!.FirstOrDefault(p => p.Id == patron.Id);
        if (existingPatron != null)
        {
            existingPatron.Name = patron.Name;
            existingPatron.ImageName = patron.ImageName;
            existingPatron.MembershipStart = patron.MembershipStart;
            existingPatron.MembershipEnd = patron.MembershipEnd;
            existingPatron.Loans = patron.Loans;
            await _jsonData.SavePatrons(_jsonData.Patrons!);
            await _jsonData.LoadData();
        }
    }
}
