using Serilog;
using DocentesApp.Data.Context;
using Microsoft.EntityFrameworkCore;
using DocentesApp.API.Configuraciones;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connString = builder.Configuration.GetConnectionString("DocentesAppDbDavaConnection");
builder.Services.AddDbContext<DocentesDbContext>(options =>
    options.UseSqlServer(connString));

builder.Services.AddAutoMapper(typeof(MapperConfig));

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
