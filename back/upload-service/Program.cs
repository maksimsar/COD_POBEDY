using Microsoft.EntityFrameworkCore;
using UploadService.Data;
using UploadService.Repositories;
using UploadService.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Сервисы
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<UploadController>();

builder.Services.AddScoped<AudioRecordRepository>();
builder.Services.AddHttpClient("SearchService", client => 
{
    client.BaseAddress = new Uri("http://localhost:5255");
});
builder.Services.AddLogging();

// База данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

// Репозитории
builder.Services.AddScoped<AudioRecordRepository>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Инициализация БД
try 
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    Console.WriteLine("Database created successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Database creation failed: {ex.Message}");
}

app.Run();