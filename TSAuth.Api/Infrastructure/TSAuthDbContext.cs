using Microsoft.EntityFrameworkCore;
using TSAuth.Api.Models;

namespace TSAuth.Api.Infrastructure;

public class TsAuthDbContext(DbContextOptions<TsAuthDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", "public");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(user => user.Name).IsRequired();
            entity.Property(user => user.Email).IsRequired();
            entity.Property(user => user.Password).IsRequired();
            entity.Property(user => user.CreatedAt).HasDefaultValueSql("now()").IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens", "public");
            entity.HasKey(token => token.Id);
            entity.Property(token => token.Id).ValueGeneratedOnAdd();
            entity.Property(token => token.UserId).IsRequired();
            entity.Property(token => token.Token).IsRequired();
            entity.Property(token => token.ExpiryDate).IsRequired();
            entity.Property(token => token.IsRevoked).HasDefaultValue(false).IsRequired();
            entity.Property(token => token.CreatedAt).HasDefaultValueSql("now()").IsRequired();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}