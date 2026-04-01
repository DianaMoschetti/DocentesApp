using Serilog;
using DocentesApp.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DocentesApp.Data.Identity;
using DocentesApp.Application.Mappings;
using DocentesApp.API.Middleware;
using Mapster;
using DocentesApp.Application.Services;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Data.Repositories;
using MapsterMapper;
using FluentValidation;
using DocentesApp.Application.Validators.Docentes;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("DocentesAppDbDavaConnection");

// Mappers
var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.Register(config);

//Context
builder.Services.AddDbContext<DocentesDbContext>(options =>
    options.UseSqlServer(connString));

// Identity
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DocentesDbContext>();


// DI Interfaces
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();

// Fluent
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateDocenteDtoValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
