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

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
// }

//app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
if (app.Environment.IsDevelopment())
    app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapFallbackToFile("index.html");

app.Run();