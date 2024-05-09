using KomalliAPI.Contexts;
using KomalliAPI.Clientes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Agregar connectionString
var connectionString = builder.Configuration.GetConnectionString("TestConnection");

// Agregar context
builder.Services.AddDbContext<KomalliIdentityContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)))
);

builder.Services.AddDbContext<KomalliContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)))
);

// Agregar servicios Identity
builder.Services.AddAuthorization();

// Activar API Identity
builder.Services.AddIdentityApiEndpoints<Cliente>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<KomalliIdentityContext>();

// Agregar servicios de controladores
builder.Services.AddControllers();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Obtener el servicio de DbContext para aplicar migraciones
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<KomalliContext>();

    // Aplicar migraciones pendientes automáticamente al iniciar la aplicación
    dbContext.Database.Migrate();
}

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
