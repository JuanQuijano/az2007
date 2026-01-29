using Library.ApplicationCore;
using Library.ApplicationCore.Entities;
using Library.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace Library.UnitTests.Infrastructure;

public class JsonLoanRepositoryTests
{
    private readonly IConfiguration _config;
    private readonly JsonData _jsonData;
    private readonly ILoanRepository _repo;

    public JsonLoanRepositoryTests()
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
        _repo = new JsonLoanRepository(_jsonData);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns loan with populated entities")]
    public async Task GetLoan_ReturnsPopulatedLoan()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        int existingLoanId = _jsonData.Loans!.First().Id;

        // Act
        var loan = await _repo.GetLoan(existingLoanId);

        // Assert
        Assert.NotNull(loan);
        Assert.Equal(existingLoanId, loan.Id);
        Assert.NotNull(loan.BookItem);
        Assert.NotNull(loan.Patron);
    }

    [Fact(DisplayName = "JsonLoanRepository.GetLoan: Returns null for non-existent loan")]
    public async Task GetLoan_ReturnsNullForNonExistentLoan()
    {
        // Arrange
        await _jsonData.EnsureDataLoaded();
        int nonExistentId = -999;

        // Act
        var loan = await _repo.GetLoan(nonExistentId);

        // Assert
        Assert.Null(loan);
    }
}
