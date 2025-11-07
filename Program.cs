using Microsoft.EntityFrameworkCore;
using CongresoTIC.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//builder.Services.AddDbContext<CongresoDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionBD")));
builder.Services.AddDbContext<CongresoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConexionBD")));
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllers();

app.Run();