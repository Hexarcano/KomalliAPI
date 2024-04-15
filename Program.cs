using KomalliAPI.Contexts;
using KomalliAPI.Clientes.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar connectionString

var connectionString = builder.Configuration.GetConnectionString("TestConnection");

// Agregar context

builder.Services.AddDbContext<KomalliIdentityContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<KomalliContext>(options => options.UseSqlServer(connectionString));

// Agregar servicios Identity

builder.Services.AddAuthorization();

// Activar API Identity

builder.Services.AddIdentityApiEndpoints<Cliente>().AddEntityFrameworkStores<KomalliIdentityContext>();

// Add services to the container.

builder.Services.AddControllers();

// Configurar swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Construir app

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapSwagger().RequireAuthorization();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
