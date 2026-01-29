var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Servir archivos estáticos
app.UseDefaultFiles();
app.UseStaticFiles();

// Ruta para la calculadora
app.MapGet("/", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.ContentRootPath, "index.html"));
});

// API endpoints para cálculos
app.MapGet("/api/calculate", (string operation, double a, double b) =>
{
    double result = operation switch
    {
        "add" => a + b,
        "subtract" => a - b,
        "multiply" => a * b,
        "divide" => b != 0 ? a / b : double.NaN,
        _ => 0
    };
    return Results.Ok(new { result });
});

app.Run();
