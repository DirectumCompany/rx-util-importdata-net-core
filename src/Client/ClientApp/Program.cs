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
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHttpClient<CommonApi>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddMudServices();
builder.Services.AddScoped<IRefreshService, RefreshService>();
builder.Services.AddScoped<InstallStore>();

string assemblyFileVersion = Assembly.GetExecutingAssembly()
    .GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
Console.WriteLine($"Client version: {assemblyFileVersion}");

await builder.Build().RunAsync();
