using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Library.UnitTests.Infrastructure;

public class JsonDataTests
{
    private readonly IConfiguration _config;
    private readonly JsonData _jsonData;

    public JsonDataTests()
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
    }

    [Fact(DisplayName = "JsonData.EnsureDataLoaded: Loads all data collections")]
    public async Task EnsureDataLoaded_LoadsAllDataCollections()
    {
        // Arrange - data not loaded yet
        Assert.Null(_jsonData.Patrons);
        Assert.Null(_jsonData.Loans);
        Assert.Null(_jsonData.Authors);
        Assert.Null(_jsonData.Books);
        Assert.Null(_jsonData.BookItems);

        // Act
        await _jsonData.EnsureDataLoaded();

        // Assert
        Assert.NotNull(_jsonData.Patrons);
        Assert.NotNull(_jsonData.Loans);
        Assert.NotNull(_jsonData.Authors);
        Assert.NotNull(_jsonData.Books);
        Assert.NotNull(_jsonData.BookItems);

        Assert.NotEmpty(_jsonData.Patrons);
        Assert.NotEmpty(_jsonData.Loans);
        Assert.NotEmpty(_jsonData.Authors);
        Assert.NotEmpty(_jsonData.Books);
        Assert.NotEmpty(_jsonData.BookItems);
    }

    [Fact(DisplayName = "JsonData.GetPopulatedPatron: Returns patron with loans collection")]
    public async Task GetPopulatedPatron_ReturnsPatronWithLoans()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        var rawPatron = _jsonData.Patrons!.First();

        // Act
        var populatedPatron = _jsonData.GetPopulatedPatron(rawPatron);

        // Assert
        Assert.NotNull(populatedPatron);
        Assert.NotNull(populatedPatron.Loans);
        Assert.Equal(rawPatron.Id, populatedPatron.Id);
        Assert.Equal(rawPatron.Name, populatedPatron.Name);
    }

    [Fact(DisplayName = "JsonData.GetPopulatedPatrons: Returns list of patrons with loans")]
    public async Task GetPopulatedPatrons_ReturnsPatronsWithLoans()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        var rawPatrons = _jsonData.Patrons!.Take(2).ToList();

        // Act
        var populatedPatrons = _jsonData.GetPopulatedPatrons(rawPatrons);

        // Assert
        Assert.NotNull(populatedPatrons);
        Assert.Equal(rawPatrons.Count, populatedPatrons.Count);
        Assert.All(populatedPatrons, p => Assert.NotNull(p.Loans));
    }
}
