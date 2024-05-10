using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Refit;
using WebApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRefitClient<IExpenseApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7011/"));
builder.Services.AddMudServices();

await builder.Build()
    .RunAsync();