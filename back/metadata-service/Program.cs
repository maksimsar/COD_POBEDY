using System.Reflection;
using Amazon.S3;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MediatR;
using MetadataService.Adapters;
using MetadataService.Application.Services;
using MetadataService.Configuration;
using MetadataService.Data;
using MetadataService.Domain.Builders;
using MetadataService.Domain.TagClassification;
using MetadataService.Infrastructure.Factories;
using MetadataService.Infrastructure.Metrics;
using MetadataService.Infrastructure.UnitOfWork;
using MetadataService.Messaging;
using MetadataService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using OpenTelemetry.Extensions.Hosting;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// ──────────────── Configuration ─────────────────
builder.Configuration
    .AddJsonFile("appsettings.Development.json", optional: false)
    .AddEnvironmentVariables();

// ──────────────── EF Core ───────────────────────
builder.Services.AddDbContext<MetadataContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default");
    opt.UseNpgsql(cs, o => o.MigrationsAssembly(typeof(MetadataContext).Assembly.FullName));
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracingBuilder => tracingBuilder
        .AddSource("MetadataService")
        .AddAspNetCoreInstrumentation())
    .WithMetrics(metricsBuilder => metricsBuilder
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
    );

// Регистрация ActivitySource / Tracer, которого будут просить ваши консьюмеры
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<TracerProvider>()
        .GetTracer("MetadataService"));

// ──────────────── MediatR + AutoMapper ──────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// ──────────────── Repositories & Builders ───────
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITranscriptRepository, TranscriptRepository>();
builder.Services.AddScoped<ISongBuilder, SongBuilder>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddSingleton<IMetrics, PrometheusMetrics>();

// ──────────────── Domain Services ───────────────
builder.Services.AddScoped<ITagClassificationService, TagClassificationService>();
builder.Services.AddSingleton<ITagClassifierFactory, TagClassifierFactory>();
builder.Services.AddSingleton<RuleBasedClassifier>();   // стратегии
builder.Services.AddSingleton<BertClassifier>();
builder.Services.AddSingleton<IDictionary<string, string[]>>(_ => new Dictionary<string, string[]>());


// ──────────────── Storage & ML Adapters ─────────
builder.Services.Configure<StorageSettings>(builder.Configuration.GetSection("Storage"));
builder.Services.Configure<SwaggerSettings>(builder.Configuration.GetSection("MlService"));

// S3 / MinIO
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddSingleton<IStorageClient, S3StorageClientAdapter>();

// HTTP-клиент к ML-сервису
builder.Services.AddHttpClient<IAudioTranscriber, AudioTranscriberHttpAdapter>((sp, c) =>
{
    var cfg = sp.GetRequiredService<IOptions<SwaggerSettings>>().Value;
    c.BaseAddress = new Uri(cfg.BaseUrl);
    c.Timeout     = TimeSpan.FromSeconds(cfg.TimeoutSeconds);
});

// ──────────────── MassTransit + EF Outbox ───────
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumers(Assembly.GetExecutingAssembly());

    // вместо RabbitMQ:
    // cfg.UsingRabbitMq((ctx, bus) => { … });

    cfg.UsingInMemory((ctx, bus) =>
    {
        bus.ConfigureEndpoints(ctx);
    });
});

// ──────────────── Controllers, Swagger, CORS ────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o =>
    o.AddPolicy("AllowAll", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

// ──────────────── Middleware pipeline ───────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
