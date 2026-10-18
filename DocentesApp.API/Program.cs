using DocentesApp.API.Extensions;
using DocentesApp.API.Middleware;
using DocentesApp.API.Services;
using DocentesApp.Application.Interfaces.Auth;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Application.Mappings;
using DocentesApp.Application.Services;
using DocentesApp.Data.Auditoria;
using DocentesApp.Data.Context;
using DocentesApp.Data.Identity;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.Validators.Docentes;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DocentesAppDbConnection");

// Mappers
var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.Register(config);

// Auditoría: usuario actual (claims del token) e interceptor del historial de cambios
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ContextoOperacionAuditoria>();
builder.Services.AddScoped<AuditoriaInterceptor>();
builder.Services.AddScoped<IAuditoriaEventos, AuditoriaEventos>();

//Context
// [Diana desde v4.0 OBSOLETO] sin interceptor de auditoría
//builder.Services.AddDbContext<DocentesDbContext>(options =>
//    options.UseSqlServer(connString));
builder.Services.AddDbContext<DocentesDbContext>((sp, options) =>
    DocentesDbContextConfiguracion.ConfigurarSqlServer(options, connString, sp.GetRequiredService<AuditoriaInterceptor>()));

// Identity

//builder.Services.AddIdentityCore<ApplicationUser>()
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<DocentesDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<DocentesDbContext>()
.AddDefaultTokenProviders();

// Jwt Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
// [Diana desde v4.0 OBSOLETO] solo fallaba si faltaba la clave, sin decir como configurarla
//var key = jwtSettings["Key"]
//    ?? throw new InvalidOperationException("JWT Key no configurada.");
// La clave JWT no se versiona (no esta en appsettings.json). En desarrollo se configura con
// dotnet user-secrets; en otros entornos con la variable de entorno Jwt__Key (ver README).
const int JwtKeyMinBytes = 32; // HMAC-SHA256 necesita una clave de al menos 256 bits
var key = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(key))
    throw new InvalidOperationException(
        "Falta la clave JWT (Jwt:Key); no se guarda en appsettings.json. " +
        "En desarrollo configurala con: dotnet user-secrets set \"Jwt:Key\" \"<clave>\" --project DocentesApp.API " +
        "(ver README). En otros entornos usa la variable de entorno Jwt__Key.");
if (Encoding.UTF8.GetByteCount(key) < JwtKeyMinBytes)
    throw new InvalidOperationException(
        $"La clave JWT (Jwt:Key) es demasiado corta: debe tener al menos {JwtKeyMinBytes} bytes (ver README).");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ClockSkew = TimeSpan.Zero
    };
    // Usuario inactivo o con security stamp distinto (desactivado, cambio de rol o contraseña): 401 aunque el token no venció
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = ValidacionTokenUsuario.ValidarAsync
    };
});

// [Diana desde v4.0 OBSOLETO] sin politica por defecto: todo lo que no tenia [Authorize] era publico
//builder.Services.AddAuthorization();
// Por defecto todos los endpoints requieren un usuario autenticado con rol Admin o User:
//   - FallbackPolicy: endpoints sin [Authorize] ni [AllowAnonymous] (las lecturas)
//   - DefaultPolicy: endpoints con [Authorize] sin roles (reportes, cargos-en-licencia)
// Las escrituras siguen con [Authorize(Roles = "Admin")]; solo el login es [AllowAnonymous].
// Swagger se sirve por middleware (UseSwagger, solo Development), no por endpoint: no le aplica.
// [Diana desde v4.0 OBSOLETO] roles Admin/User reemplazados por Administrador/Editor/Consultante y políticas con nombre
//var politicaAdminOUser = new AuthorizationPolicyBuilder()
//    .RequireAuthenticatedUser()
//    .RequireRole("Admin", "User")
//    .Build();
//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = politicaAdminOUser;
//    options.DefaultPolicy = politicaAdminOUser;
//});
// Políticas compartidas con Blazor (DocentesApp.Shared/Auth/Politicas.cs):
//   - PuedeConsultar: cualquier rol. Es la Fallback (lecturas sin atributo) y la Default ([Authorize] a secas: reportes)
//   - PuedeEditar: Administrador o Editor (todas las escrituras, incluidos los catálogos)
//   - EsAdministrador: gestión de usuarios
builder.Services.AddAuthorization(options =>
{
    Politicas.Configurar(options);
    options.FallbackPolicy = options.GetPolicy(Politicas.PuedeConsultar);
    options.DefaultPolicy = options.GetPolicy(Politicas.PuedeConsultar)!;
});
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>(); // gestión de usuarios (api/usuarios)
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>(); // lectura del historial de cambios (api/auditoria)

// DI Interfaces
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
// [Diana desde v4.0 OBSOLETO] Cargo reemplazado por PuntosPorCargo
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
// [Diana desde v4.0 OBSOLETO] Dedicacion reemplazada por atributos en DetalleDesignacion
builder.Services.AddScoped<IDedicacionService, DedicacionService>();
builder.Services.AddScoped<IDedicacionRepository, DedicacionRepository>();
builder.Services.AddScoped<IPuntosPorCargoService, PuntosPorCargoService>();
builder.Services.AddScoped<IPuntosPorCargoRepository, PuntosPorCargoRepository>();
builder.Services.AddScoped<IUdbService, UdbService>();
builder.Services.AddScoped<IUdbRepository, UdbRepository>();
builder.Services.AddScoped<IAsignaturaService, AsignaturaService>();
builder.Services.AddScoped<IAsignaturaRepository, AsignaturaRepository>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IDesignacionService, DesignacionService>();
builder.Services.AddScoped<IDesignacionRepository, DesignacionRepository>();
// Reportes
builder.Services.AddScoped<IPlantaReporteService, PlantaReporteService>();
builder.Services.AddScoped<IPlantaReporteRepository, PlantaReporteRepository>();

// Fluent
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateDocenteDtoValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DocentesApp API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingres� el token JWT: Bearer {tu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//ctx: for configuration file, lc: for logger configuration
builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b =>
    {
        b.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling(); // para utilizar el middleware de las excepciones

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.SeedIdentityDataAsync();
await app.SeedDocentesAsync();
await app.SeedUdbsAsync();
await app.SeedDocenteUdbAsync();
//await app.SeedAsignaturasAsync();
await app.SeedAsignaturasAsync();

app.Run();

public partial class Program { } // para que el WebApplicationFactory<Program> del proyecto de pruebas pueda acceder a program.cs y ejecutar la aplicaci�n en memoria
