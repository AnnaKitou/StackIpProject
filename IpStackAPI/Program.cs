using IpStackAPI.Context;
using IpStackAPI.FactoryPattern;
using IpStackAPI.GenericRepository;
using IpStackAPI.Interfaces;
using IpStackAPI.RepositoryServices;
using Microsoft.EntityFrameworkCore;
using StackIpProject;
using StackIpProject.Configuration;
using StackIpProject.Interfaces;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Configuration.SetBasePath(System.AppContext.BaseDirectory)
                  .AddJsonFile(Path.Combine("appsettings2.json"), optional: false, reloadOnChange: true);
builder.Services.Configure<EndPointSetting>(options =>
builder.Configuration.GetSection("EndPointSetting").Bind(options));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddScoped<IStackIpService, StackIpService>();


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IIPInfoProvider, IPInfoProvider>();
builder.Services.AddScoped<IBatchUpdateService, BatchUpdateService>();
builder.Services.AddSingleton<IBatchUpdateServiceFactory, BatchUpdateServiceFactory>();
builder.Services.AddHostedService<BatchUpdateBackgroundService>();
builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
