<h1 align="center">.NET 8 Web API</h1>

<p align="center">
  <img src="assets/Background.jpg" alt="logo"/>
  <br>
  <em>Building a Web API with .NET 8</em>
  <br>
</p>

# Setup

The following tools are required to build and run this project:
 * [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
 * A Code editor like [Visual Studio Code](https://code.visualstudio.com/)
 * A browser of your choice that supports JavaScript and WebAssembly (so everything except Internet Explorer 11)

# What is in the box?
There are three projects in this repository:
 * **ExpenseTracker**: A .NET 8 Web API project that serves as the backend for the frontend project.
 * **Frontend**: A Blazor WebAssembly project that serves as the frontend for the Web API project.
 * **ExpenseTracker.Tests**: A .NET 8 xUnit project that contains unit tests for the Web API project (WebApplicationFactory).
  
The whole application is a simple Expense Tracker (CRUD application) that allows you to add, edit, delete and view expenses.
It uses a **SQLite** database to store the expenses (via Entity Framework Core) and **SignalR** to notify the frontend when the expenses are updated.

There are two branches:
 * **main**: Contains the final version of the application. So everything is already implemented.
 * **ui-only**: Contains the frontend project only. So you can implement the backend yourself.

But it would be nice to understand the single steps - therefore there are some branches that incrementally go from `ui-only` to `main`:
 * **step-1**: This branch adds the Web API project and the first controller via Minimal API. The controller returns a static list of expenses. It also includes CORS to make the frontend work!.
 * **step-2**: Adding **Entity Framework** with **SQLite** to enable retrieval via the database.
 * **step-3**: Implementing the POST request, so that the client can easily add new expenses and return them.
 * **step-4**: Implementing the PUT request, so that the client can easily update existing expenses.
 * **step-5**: Implementing the DELETE request, so that the client can easily delete existing expenses.
 * **step-6**: Moving everything to its own small file to cleanup "the mess".
 * **step-7**: Adding **SignalR** to notify the frontend when the expenses are updated.
 * **step-8**: Adding logging and our own first middleware
 * **step-9**: Adding the global exception handler
 * **step-10**: Adding the Tests (WebApplicationFactory)