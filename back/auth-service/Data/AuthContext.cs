using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public sealed class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options) { }
    
    public DbSet<User>         Users         => Set<User>();
    public DbSet<Role>         Roles         => Set<Role>();
    public DbSet<UserRole>     UserRoles     => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email)
                .HasColumnType("citext")
                .HasMaxLength(100)
                .IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn();
            entity.Property(r => r.Name).IsRequired();
            entity.HasIndex(r => r.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId);
            
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.HasKey(rt => rt.Token);

            entity.Property(rt => rt.IpAddress)
                .HasColumnType("inet");
            
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
        });
        
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(om => om.Id);
            entity.Property(om => om.Id)
                .ValueGeneratedOnAdd();                  
            entity.Property(om => om.Type)
                .IsRequired();
            entity.Property(om => om.Payload)
                .IsRequired();
            entity.Property(om => om.OccurredOnUtc)
                .HasColumnType("timestamp with time zone");
            entity.Property(om => om.Processed)
                .HasDefaultValue(false);
        });
    }
}