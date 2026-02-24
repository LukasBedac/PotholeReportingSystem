using GUIApps.Shared.Services;
using GUIApps.Shared.Services.Interfaces;
using GUIApps.Web.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the GUIApps.Shared project
//builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Register the State Provider that reads the server data
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddScoped<GUIApps.Shared.Services.Interfaces.IAuthService, GUIApps.Shared.Services.WebAuthService>();

// Register the Auth Service (same logic as server: redirect to controller)
// Since the Controller is on the Server, the WASM client simply does a full page reload to hit it.
builder.Services.AddScoped<IAuthService, WebAuthService>();
builder.Services.AddMudServices();

// Register HttpClients for WebAssembly (accessing services via browser/localhost)
builder.Services.AddHttpClient("FileService", client => 
    client.BaseAddress = new Uri("http://localhost:8085"));
builder.Services.AddHttpClient("ReportService", client => 
    client.BaseAddress = new Uri("http://localhost:8081"));


await builder.Build().RunAsync();
