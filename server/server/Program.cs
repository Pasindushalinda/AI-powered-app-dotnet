var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/api/hello", () => Results.Json(new { message = "Hello World!" }));

app.Run();