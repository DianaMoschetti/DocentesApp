using Serilog;
using DocentesApp.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DocentesApp.Data.Identity;
using DocentesApp.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connString = builder.Configuration.GetConnectionString("DocentesAppDbDavaConnection");
builder.Services.AddDbContext<DocentesDbContext>(options =>
    options.UseSqlServer(connString));

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DocentesDbContext>();

// Mappers
// Agrega AutoMapper y registra los perfiles de mapeo.
// El método AddAutoMapper escanea el ensamblado especificado (en este caso, donde se encuentra DocenteProfile)
// en busca de clases que hereden de Profile y las registra automáticamente.
//el .Assebly busca en todos los profile q esten dentro del mismo proy donde esta docenteProfile
builder.Services.AddAutoMapper(typeof(DocenteProfile).Assembly); 

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
