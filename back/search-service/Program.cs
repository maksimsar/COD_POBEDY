using Nest;
using SearchService.Services;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddEnvironmentVariables();

// Сервисы
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Elasticsearch
var elasticUrl = builder.Configuration["Elasticsearch:Url"];
var settings = new ConnectionSettings(new Uri(elasticUrl))
    .DefaultIndex("audio_records")
    .EnableDebugMode();

builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));
builder.Services.AddScoped<ElasticSearchService>();

var app = builder.Build();

// Инициализация индекса
using (var scope = app.Services.CreateScope())
{
    var client = scope.ServiceProvider.GetRequiredService<IElasticClient>();
    var response = client.Indices.Create("audio_records", c => c
    .Map<AudioRecord>(m => m
        .Properties(p => p
            .Text(t => t
                .Name(n => n.SongName)
                .Analyzer("russian") 
                .Fields(f => f
                    .Keyword(k => k.Name("keyword"))
                    .Text(tt => tt.Name("russian").Analyzer("russian"))
                )
            )
            .Keyword(k => k.Name(n => n.Author))
        )
    )
);
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();