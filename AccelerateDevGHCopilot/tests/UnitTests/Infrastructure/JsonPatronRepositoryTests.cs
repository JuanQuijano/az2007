using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Library.UnitTests.Infrastructure;

public class JsonPatronRepositoryTests
{
    private readonly IConfiguration _config;
    private readonly JsonData _jsonData;
    private readonly IPatronRepository _repo;

    public JsonPatronRepositoryTests()
    {
        // Find solution root by looking for .sln file
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !dir.GetFiles("*.sln").Any())
        {
            dir = dir.Parent;
        }
        var jsonDir = Path.Combine(dir!.FullName, "src", "Library.Console", "Json");
        
        // Provide absolute paths to JSON files
        var configData = new Dictionary<string, string?>
        {
            ["JsonPaths:Authors"] = Path.Combine(jsonDir, "Authors.json"),
            ["JsonPaths:Books"] = Path.Combine(jsonDir, "Books.json"),
            ["JsonPaths:BookItems"] = Path.Combine(jsonDir, "BookItems.json"),
            ["JsonPaths:Patrons"] = Path.Combine(jsonDir, "Patrons.json"),
            ["JsonPaths:Loans"] = Path.Combine(jsonDir, "Loans.json")
        };
        
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _jsonData = new JsonData(_config);
        _repo = new JsonPatronRepository(_jsonData);
    }

    [Fact(DisplayName = "JsonPatronRepository.GetPatron: Returns patron with populated loans")]
    public async Task GetPatron_ReturnsPopulatedPatron()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        int existingPatronId = _jsonData.Patrons!.First().Id;

        // Act
        var patron = await _repo.GetPatron(existingPatronId);

        // Assert
        Assert.NotNull(patron);
        Assert.NotNull(patron.Loans);
        Assert.Equal(existingPatronId, patron.Id);
    }

    [Fact(DisplayName = "JsonPatronRepository.GetPatron: Returns null for non-existent patron")]
    public async Task GetPatron_ReturnsNullForNonExistentPatron()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        int nonExistentId = -999;

        // Act
        var patron = await _repo.GetPatron(nonExistentId);

        // Assert
        Assert.Null(patron);
    }

    [Fact(DisplayName = "JsonPatronRepository.SearchPatrons: Returns matching patrons sorted by name")]
    public async Task SearchPatrons_ReturnsMatchingPatronsSortedByName()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        string searchTerm = _jsonData.Patrons!.First().Name.Substring(0, 3);

        // Act
        var results = await _repo.SearchPatrons(searchTerm);

        // Assert
        Assert.NotNull(results);
        Assert.All(results, p => Assert.Contains(searchTerm, p.Name));
        // Verify sorted by name
        for (int i = 1; i < results.Count; i++)
        {
            Assert.True(string.Compare(results[i - 1].Name, results[i].Name) <= 0);
        }
    }

    [Fact(DisplayName = "JsonPatronRepository.SearchPatrons: Returns empty list for no matches")]
    public async Task SearchPatrons_ReturnsEmptyListForNoMatches()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        string nonMatchingTerm = "XYZNONEXISTENT999";

        // Act
        var results = await _repo.SearchPatrons(nonMatchingTerm);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }
}
