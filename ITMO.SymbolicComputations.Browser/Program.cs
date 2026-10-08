using ITMO.SymbolicComputations.Browser;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

// The existing HTML interface owns rendering. Blazor only hosts the genuine C# engine.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
var host = builder.Build();
await host.Services.GetRequiredService<IJSRuntime>().InvokeVoidAsync(
    "symbolicManagedStarted", typeof(BrowserApi).Assembly.GetName().Name);
await host.RunAsync();
