using Microsoft.EntityFrameworkCore;
using UploadService.Models;

namespace UploadService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AudioRecord> AudioRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AudioRecord>().HasIndex(a => a.SongName);
    }
}