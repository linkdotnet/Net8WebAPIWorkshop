using Refit;

namespace WebApp;

public interface IExpenseApi
{
    [Get("/api/expenses")]
    Task<IReadOnlyCollection<Expense>> GetAll(int? page, int? pageSize);
    
    [Post("/api/expenses")]
    Task Create([Body] Expense expense);
    
    [Put("/api/expenses/{id}")]
    Task Update(int id, [Body] Expense expense);
    
    [Delete("/api/expenses/{id}")]
    Task Delete(int id);
}

public record Expense
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public decimal Value { get; set; }

    public string[] Categories { get; set; } = [];

    public DateOnly ExpenseDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}