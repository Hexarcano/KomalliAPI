using KomalliAPI.Contexts;
using KomalliAPI.Clientes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Agregar connectionString

var connectionString = builder.Configuration.GetConnectionString("TestConnection");

// Agregar context

builder.Services.AddDbContext<KomalliIdentityContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<KomalliContext>(options => options.UseSqlServer(connectionString));

// Agregar Jwt Authentication

builder.Services.AddSingleton<ITokenRevocationService, TokenRevocationService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

// Agregar servicios Identity

builder.Services.AddAuthorization();

// Activar API Identity

builder.Services.AddIdentityApiEndpoints<Cliente>()
    .AddRoles<ClienteRol>()
    .AddEntityFrameworkStores<KomalliIdentityContext>();

// Inicializar Roles

using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ClienteRol>>();

    await SeedRoles(roleManager);
}

// Add services to the container.

builder.Services.AddControllers();

// Configurar swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Construir app

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapSwagger().RequireAuthorization();
}

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


async Task SeedRoles(RoleManager<ClienteRol> roleManager)
{
    var roles = Enum.GetValues(typeof(ERol));

    foreach (var rol in roles)
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(rol.ToString()))
            {
                var nuevoRol = new ClienteRol(rol.ToString());

                await roleManager.CreateAsync(nuevoRol);
            }
        }
        catch (InvalidOperationException ex)
        {
            Debug.WriteLine("Error al conectar con la base de datos, puede que sea por la creación de la migration");
        }
    }
}
