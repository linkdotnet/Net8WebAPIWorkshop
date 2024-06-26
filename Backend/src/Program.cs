using ExpenseTracker;
using ExpenseTracker.Controller;
using ExpenseTracker.Infrastructure;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

builder.Services.AddSignalR();
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlite("Data Source=app.db");
});

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("OnlyUs", b =>
    {
        b.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://localhost:7179");
    });
});

// Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Services.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(_ => { });

// Add our middleware
/*
 * Browser           Middleware           Controller/Action
   |                 |                  |
   |---Request------>|                  |
   |                 |---Request------->|
   |                 |<--Response-------|
   |                 X                  X
   |<--Response------|                  |
   |                 |                  |
   |---Request------>|                  |
   |                 |---Request------->|
   |                 |<--Exception------|
   |<--Error Object--|                  |
 */
app.Use(async (context, next) =>
{
    var logger = app.Logger;
    logger.LogDebug("Before Request with Uri '{Uri}'", context.Request.GetDisplayUrl());
    await next(context);
    logger.LogDebug("After Request with Uri '{Uri}'", context.Request.GetDisplayUrl());
});

app.UseCors("OnlyUs");

app.RegisterExpenseEndpoints();

app.MapHub<ExpenseHub>("expensehub");

app.Run();

public partial class Program;