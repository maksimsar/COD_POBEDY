using Microsoft.EntityFrameworkCore;
using MetadataService.Models;
using MassTransit.EntityFrameworkCoreIntegration;

namespace MetadataService.Data;

public sealed class MetadataContext : DbContext
{
    public MetadataContext(DbContextOptions<MetadataContext> options) : base(options)
    {
    }
    
    public DbSet<Song>       Songs       => Set<Song>();
    public DbSet<Author>     Authors     => Set<Author>();
    public DbSet<SongAuthor> SongAuthors => Set<SongAuthor>();
    public DbSet<Tag>        Tags        => Set<Tag>();
    public DbSet<SongTag>    SongTags    => Set<SongTag>();
    public DbSet<Transcript> Transcripts => Set<Transcript>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    
    // OnConfiguring оставлен как запасной вариант, если DI не настроит опции (например, при выполнении миграций)
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
        
        //Song
        modelBuilder.Entity<Song>(entity =>
        {
            entity.ToTable("songs");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");

            entity.Property(s => s.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(s => s.Description)
                .HasColumnName("description")
                .HasMaxLength(1024);

            entity.Property(s => s.Year)
                .HasColumnName("year");

            entity.Property(s => s.DurationSec)
                .HasColumnName("duration_sec");

            entity.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            entity.Property(s => s.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("now()");

            entity.HasIndex(s => s.Title)
                .HasDatabaseName("ix_songs_title");

        });
        
        //Author
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("authors");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("uuid_generate_v4()");

            entity.Property(a => a.FullName)
                .HasColumnName("full_name")
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.BirthYear)
                .HasColumnName("birth_year");

            entity.Property(a => a.DeathYear)
                .HasColumnName("death_year");

            entity.Property(a => a.Notes)
                .HasColumnName("notes")
                .HasMaxLength(1000);
        });
        
        //Song_author
        modelBuilder.Entity<SongAuthor>(entity =>
        {
            entity.ToTable("song_authors");
            entity.HasKey(sa => new { sa.SongId, sa.AuthorId, sa.Role });

            entity.Property(sa => sa.SongId)
                .HasColumnName("song_id");

            entity.Property(sa => sa.AuthorId)
                .HasColumnName("author_id");

            entity.Property(sa => sa.Role)
                .HasColumnName("role")
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(sa => sa.Song)
                .WithMany(s => s.SongAuthors)
                .HasForeignKey(sa => sa.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sa => sa.Author)
                .WithMany(a => a.SongAuthors)
                .HasForeignKey(sa => sa.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        //Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .HasColumnName("id")
                .UseIdentityColumn();

            entity.Property(t => t.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Approved)
                .HasColumnName("approved");

            entity.HasIndex(t => t.Name)
                .IsUnique()
                .HasDatabaseName("ux_tags_name");
        });
        
        //Song tag
        modelBuilder.Entity<SongTag>(entity =>
        {
            entity.ToTable("song_tags");
            entity.HasKey(st => new { st.SongId, st.TagId });

            entity.Property(st => st.SongId)
                .HasColumnName("song_id");

            entity.Property(st => st.TagId)
                .HasColumnName("tag_id");

            entity.HasOne(st => st.Song)
                .WithMany(s => s.SongTags)
                .HasForeignKey(st => st.SongId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(st => st.Tag)
                .WithMany(t => t.SongTags)
                .HasForeignKey(st => st.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        //Transcript
        modelBuilder.Entity<Transcript>(entity =>
        {
            entity.ToTable("transcripts");
            entity.HasKey(tr => tr.Id);

            entity.Property(tr => tr.Id)
                .HasColumnName("id")
                .UseIdentityColumn();

            entity.Property(tr => tr.SongId)
                .HasColumnName("song_id");

            entity.Property(tr => tr.SegmentIndex)
                .HasColumnName("segment_index")
                .IsRequired();

            entity.Property(tr => tr.StartMs)
                .HasColumnName("start_ms")
                .IsRequired();

            entity.Property(tr => tr.EndMs)
                .HasColumnName("end_ms")
                .IsRequired();

            entity.Property(tr => tr.Text)
                .HasColumnName("text")
                .IsRequired();

            entity.Property(tr => tr.Confidence)
                .HasColumnName("confidence");

            entity.Property(tr => tr.CheckedById)
                .HasColumnName("checked_by_id");

            entity.Property(tr => tr.CheckedAt)
                .HasColumnName("checked_at");

            entity.HasIndex(tr => new { tr.SongId, tr.SegmentIndex })
                .IsUnique()
                .HasDatabaseName("ux_transcripts_song_segment");

            entity.HasOne(tr => tr.Song)
                .WithMany(s => s.Transcripts)
                .HasForeignKey(tr => tr.SongId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}