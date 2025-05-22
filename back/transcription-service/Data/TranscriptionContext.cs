using Microsoft.EntityFrameworkCore;
using TranscriptionService.Messaging.Contracts;
using TranscriptionService.Models;

namespace TranscriptionService.Data;

public sealed class TranscriptionContext : DbContext
{
    public TranscriptionContext(DbContextOptions<TranscriptionContext> options) : base(options)
    {
    }

    public DbSet<TranscriptionJob> TranscriptionJobs => Set<TranscriptionJob>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Создаём конфигурацию вручную из appsettings.json, который находится в корне приложения
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Developmetn.json", optional: false, reloadOnChange: true)
                .Build();

            // Читаем строку подключения по ключу "DefaultConnection"
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Настраиваем подключение к PostgreSQL
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<TranscriptionJob>(entity =>
        {
            entity.ToTable("transcription_jobs");
            
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id)
                .UseIdentityColumn()
                .HasColumnName("id");
            
            entity.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone");

            entity.Property(t => t.AudioFileId)
                .HasColumnName("audio_file_id")
                .IsRequired();

            entity.Property(t => t.ModelUsed)
                .HasColumnName("model_used")
                .IsRequired()
                .HasMaxLength(64);
            entity.Property(t => t.StartedAt)
                .HasColumnName("started_at")
                .HasColumnType("timestamp without time zone");
            entity.Property(t => t.FinishedAt)
                .HasColumnName("finished_at")
                .HasColumnType("timestamp without time zone");
            entity.Property(t => t.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasMaxLength(16);

            entity.Property(t => t.ErrorMessage)
                .HasColumnName("error_message");
            // Логический FK на файл — для ускорения запросов
            entity.HasIndex(t => t.AudioFileId)
                .HasDatabaseName("ix_transcription_jobs_audio_file_id");
        });

        modelBuilder.Entity<OutboxMessage>(entitiy =>
        {
            entitiy.ToTable("outbox_messages");

            entitiy.HasKey(m => m.Id);
            entitiy.Property(m => m.Id)
                .UseIdentityColumn()
                .HasColumnName("id");

            entitiy.Property(m => m.Type)
                .HasColumnName("type")
                .IsRequired();

            entitiy.Property(m => m.Payload)
                .HasColumnName("payload")
                .IsRequired()
                .HasColumnType("jsonb");

            entitiy.Property(m => m.OccurredOnUtc)
                .HasColumnName("occurred_on_utc")
                .HasColumnType("timestamp with time zone");

            entitiy.Property(m => m.Processed)
                .HasColumnName("processed")
                .IsRequired();

            // Используем filtered-index, чтобы быстрее подхватывать непроцессed записи
            entitiy.HasIndex(m => m.Processed)
                .HasDatabaseName("ix_outbox_processed");
        });
    }
}