using ExpenseTracker.Domain;

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

app.MapGet("/api/expenses", () =>
    {
        var expense = new Expense("Expense 1", 100m, [""], new DateOnly(2024, 5, 14));
        return new[] { expense };
    })
    .WithDescription("Retrieves a list of expenses")
    .WithSummary("Get all expenses");

app.Run();