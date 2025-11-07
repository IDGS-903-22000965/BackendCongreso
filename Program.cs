using Microsoft.EntityFrameworkCore;
using CongresoTIC.API.Data;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddControllers();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("ConexionBD");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión a la base de datos");
}

Console.WriteLine($"URL original (longitud: {connectionString.Length})");

if (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
{
    try
    {
        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':');
        var dbPort = uri.Port > 0 ? uri.Port : 5432; 

        connectionString = $"Host={uri.Host};Port={dbPort};Database={uri.AbsolutePath.Trim('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

        Console.WriteLine("URL convertida exitosamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al convertir URL: {ex.Message}");
        throw;
    }
}

builder.Services.AddDbContext<CongresoDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<CongresoDbContext>();
        db.Database.Migrate();
        Console.WriteLine("Migraciones aplicadas exitosamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al aplicar migraciones: {ex.Message}");
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();