using System.Diagnostics;
using ImportUtilServer.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<Import>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
}

app.MapRazorPages();
app.MapControllers();
if (app.Environment.IsDevelopment())
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapFallbackToFile("index.html");

var runningApp = app.RunAsync();
if (!app.Environment.IsDevelopment())
{
    var url = app.Urls.FirstOrDefault(u => u.StartsWith("http://"));
    if (url != null)
        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
}
await runningApp;