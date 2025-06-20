#:sdk Microsoft.Sdk.NET.Web

var builder = WebApplication.CreateBuilder();
var app = builder.Build();
app.MapGet("/", () => "Hello World");
app.Run();
