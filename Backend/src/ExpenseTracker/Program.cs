using ExpenseTracker.Domain;
using ExpenseTracker.Infrastructure;
using Microsoft.EntityFrameworkCore;

/*
 * +-------+         +---------+          +-------------+          +-------+
   | User  |         | Frontend|          | ASP.NET Core|          |SQLite |
   |       |         |         |          | Backend     |          |DB     |
   +-------+         +---------+          +-------------+          +-------+
   |                 |                      |                      |
   |1.Enter Expense  |                      |                      |
   |---------------->|                      |                      |
   |                 |                      |                      |
   |                 |2. HTTP POST Request  |                      |
   |                 |--------------------->|                      |
   |                 |                      |                      |
   |                 |                      |3. Save Expense via   |
   |                 |                      |   Entity Framework   |
   |                 |                      |--------------------->|
   |                 |                      |                      |
   |                 |                      |    4. Expense Saved  |
   |                 |                      |<---------------------|
   |                 |                      |                      |
   |                 |   5. HTTP 200 OK     |                      |
   |                 |<---------------------|                      |
   |                 |                      |                      |
   |6. Confirmation  |                      |                      |
   |<----------------|                      |                      |
   |                 |                      |                      |
 */
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlite("Data Source=app.db");
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("OnlyUs", b =>
    {
        b.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://localhost:7179");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("OnlyUs");

app.MapGet("/", async (AppDbContext dbContext, int? page, int? pageSize) =>
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

app.MapPost("/", async (AppDbContext dbContext, CreateExpenseDto createExpenseDto) =>
    {
        var expense = new Expense(
            createExpenseDto.Name,
            createExpenseDto.Value,
            createExpenseDto.Categories,
            createExpenseDto.ExpenseDate);
            
        await dbContext.Expenses.AddAsync(expense);
        await dbContext.SaveChangesAsync();
    })
    .WithDescription("Creates a new expense");

app.MapPut("/{id:int}", async (int id, UpdateExpenseDto dto, AppDbContext dbContext) =>
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
    })
    .WithDescription("Updates an expense");
app.Run();

record CreateExpenseDto(string Name, decimal Value, string[] Categories, DateOnly ExpenseDate);
record UpdateExpenseDto(string Name, decimal Value, string[] Categories, DateOnly ExpenseDate);