using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Domain;
using ExpenseTracker.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace ExpenseTracker.Controller;

public static class ExpenseExtensions
{
    public static void RegisterExpenseEndpoints(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/api/expenses");

        group.MapGet("/", async (AppDbContext dbContext, int? page, int? pageSize) =>
            {
                page ??= 0;
                pageSize ??= int.MaxValue;
                return await dbContext
                    .Expenses
                    .OrderBy(b => b.Id)
                    .Skip(page.Value)
                    .Take(pageSize.Value)
                    .AsNoTracking()
                    .ToListAsync();
            })
            .WithDescription("Retrieves a list of expenses")
            .WithSummary("Get all expenses");
        
        group.MapPost("/", async (AppDbContext dbContext, CreateExpenseDto createExpenseDto, IHubContext<ExpenseHub> hub) =>
        {
            var expense = new Expense(
                createExpenseDto.Name,
                createExpenseDto.Value,
                createExpenseDto.Categories,
                createExpenseDto.ExpenseDate);
            
            await dbContext.Expenses.AddAsync(expense);
            await dbContext.SaveChangesAsync();
            await NotifyUpdate(hub);
        })
        .WithDescription("Creates a new expense");
        
        group.MapDelete("/{id:int}", async (int id, AppDbContext dbContext, IHubContext<ExpenseHub> hub) =>
        {
            var expense = await dbContext.Expenses.FindAsync(id)
                ?? throw new InvalidOperationException($"Can't find expense with id {id}");
            dbContext.Expenses.Remove(expense);
            await dbContext.SaveChangesAsync();
            
            await NotifyUpdate(hub);
        })
        .WithDescription("Deletes an expense");

        group.MapPut("/{id:int}", async (int id, UpdateExpenseDto dto, AppDbContext dbContext, IHubContext<ExpenseHub> hub) =>
        {
            var update = new Expense(
                dto.Name,
                dto.Value,
                dto.Categories,
                dto.ExpenseDate);
            
            var expense = await dbContext.Expenses.FindAsync(id) 
                          ?? throw new InvalidOperationException($"Can't find expense with id {id}");
            
            expense.Update(update);
            await dbContext.SaveChangesAsync();
            await NotifyUpdate(hub);
        })
        .WithDescription("Updates an expense");

        group.WithOpenApi();
    }

    private static async Task NotifyUpdate(IHubContext<ExpenseHub> hub)
    {
        await hub.Clients.All.SendAsync("update");
    }

    private record CreateExpenseDto(string Name, decimal Value, string[] Categories, DateOnly ExpenseDate);

    private record UpdateExpenseDto(string Name, decimal Value, string[] Categories, DateOnly ExpenseDate);
}