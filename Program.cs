using KomalliAPI.Contexts;
using KomalliAPI.Clientes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Agregar connectionString

var connectionString = builder.Configuration.GetConnectionString("TestConnection");

// Agregar context

builder.Services.AddDbContext<KomalliIdentityContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<KomalliContext>(options => options.UseSqlServer(connectionString));

// Agregar servicios Identity

builder.Services.AddAuthorization();

// Activar API Identity

builder.Services.AddIdentityApiEndpoints<Cliente>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<KomalliIdentityContext>();


// Inicializar Roles

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedRoles(roleManager);
}

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


async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = Enum.GetValues(typeof(Rol));

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol.ToString()))
        {
            var nuevoRol = new IdentityRole(rol.ToString());
            await roleManager.CreateAsync(nuevoRol);
        }
    }
}
