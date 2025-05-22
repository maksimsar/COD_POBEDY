using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TranscriptionService.Adapters;
using TranscriptionService.Configuration;
using TranscriptionService.Data;
using TranscriptionService.Infrastructure;
using TranscriptionService.Interfaces;
using TranscriptionService.Messaging.Consumers;
using TranscriptionService.Repositories;
using TranscriptionService.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;
var svc = builder.Services;

// ──────────────── Configuration ─────────────────
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: false)
    .AddEnvironmentVariables();

// ──────────────── EF Core + Postgres ────────────
svc.AddDbContext<TranscriptionContext>(opt =>
{
    var cs = cfg.GetConnectionString("Postgres");
    opt.UseNpgsql(cs, o => o.MigrationsAssembly(typeof(TranscriptionContext).Assembly.FullName));
});


// ──────────────── AutoMapper ─────────────────────
svc.AddAutoMapper(Assembly.GetExecutingAssembly());

// ──────────────── Repositories & Unit of Work ────
svc.AddScoped<ITranscriptionJobRepository, TranscriptionJobRepository>();

// ──────────────── Storage Adapter ────────────────
svc.Configure<StorageSettings>(cfg.GetSection("Storage"));
svc.AddSingleton<IStorageClient, MinioStorageClient>();

// ──────────────── Whisper HTTP Adapter ──────────
svc.AddHttpClient<ISttEngine, WhisperHttpAdapter>(c =>
{
    c.BaseAddress = new Uri(cfg["SttEngine:BaseUrl"]!);
    c.Timeout = TimeSpan.FromSeconds(cfg.GetValue<int>("SttEngine:TimeoutSeconds", 30));
});

// ──────────────── Domain Service & Background Queue
svc.AddScoped<ITranscriptionService, TranscriptionServiceImpl>();
svc.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
svc.AddHostedService<BackgroundTaskWorker>();

// ──────────────── MassTransit + EF Outbox ───────
svc.AddMassTransit(x =>
{
    x.AddConsumer<FileUploadedConsumer>();

    x.AddEntityFrameworkOutbox<TranscriptionContext>(ob =>
    {
        ob.QueryDelay = TimeSpan.FromSeconds(10);
        ob.UsePostgres();
        ob.UseBusOutbox();
    });

    x.UsingRabbitMq((ctx, mq) =>
    {
        mq.Host(cfg["Rabbit:Host"]!, "/", h =>
        {
            h.Username(cfg["Rabbit:User"] ?? "guest");
            h.Password(cfg["Rabbit:Pass"] ?? "guest");
        });
        mq.ConfigureEndpoints(ctx);
    });
});

// ──────────────── Controllers, Swagger, CORS ─────
svc.AddControllers();
svc.AddEndpointsApiExplorer();
svc.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transcription Service",
        Version = "v1"
    });
});
svc.AddCors(o =>
    o.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// ──────────────── HTTP pipeline ─────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseRouting();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
