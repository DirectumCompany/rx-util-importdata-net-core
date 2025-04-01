using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using ClientApp;
using ClientApp.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using ClientApp.Store;
using ClientApp.Services;
using Microsoft.AspNetCore.Components.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
string uri = builder.HostEnvironment.IsDevelopment()
    ? "http://localhost:5000/"
    : builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient<CommonApi>(client => client.BaseAddress = new Uri(uri));
builder.Services.AddMudServices();
builder.Services.AddScoped<IRefreshService, RefreshService>();
builder.Services.AddScoped<InstallStore>();
builder.RootComponents.Add<HeadOutlet>("head::after");

string assemblyFileVersion = Assembly.GetExecutingAssembly()
    .GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
Console.WriteLine($"Client version: {assemblyFileVersion}");

await builder.Build().RunAsync();
