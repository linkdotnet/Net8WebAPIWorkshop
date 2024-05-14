using System.Net;
using System.Net.Http.Json;
using ExpenseTracker.Domain;
using ExpenseTracker.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Tests;

public class ExpenseTrackerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    public ExpenseTrackerTests(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }
    
    [Fact]
    public async Task WhenAddingAnExpense_ItShouldBeStored()
    {
        using var client = _webApplicationFactory
                // Replace our production database with a in memory database
            .WithWebHostBuilder(c => c.ConfigureTestServices(
                s =>
                {
                    // Override DbContext via AddScoped as AddDbContext doesn't work
                    // as it uses TryAddDbContext internally
                    s.AddScoped<AppDbContext>(_ =>
                    {
                        var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseSqlite("DataSource=file::memory:?cache=shared")
                            .Options;
                        return new AppDbContext(options);
                    });
                }))
            .CreateClient();

        var result = await client.PostAsJsonAsync("api/expenses", new
        {
            Name = "Expense 1",
            Value = 100m,
            Categories = new[] {"Cat 1"},
            ExpenseDate = new DateOnly(2024, 5, 14)
        });
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var expenses = await client.GetFromJsonAsync<Expense[]>("api/expenses");

        Assert.NotNull(expenses);
        Assert.Single(expenses);
        var expense = expenses.Single();
        Assert.Equal("Expense 1", expense.Name);
        Assert.Equal(100m, expense.Value);
        Assert.Equal("Cat 1",  expense.Categories.Single());
        Assert.Equal(new DateOnly(2024, 5, 14),  expense.ExpenseDate);
    }
}