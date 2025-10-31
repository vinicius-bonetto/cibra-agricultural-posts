using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cibra.AgriculturalPosts.Domain.Entities;

namespace Cibra.AgriculturalPosts.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Posts");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedNever();

            entity.Property(p => p.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(p => p.Location)
                .HasMaxLength(200);

            entity.Property(p => p.Status)
                .HasConversion<string>();

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            // Complex type for Analysis
            entity.OwnsOne(p => p.Analysis, analysis =>
            {
                analysis.Property(a => a.CultureType)
                    .HasMaxLength(100);

                analysis.Property(a => a.Stage)
                    .HasConversion<string>();

                analysis.Property(a => a.RawAIResponse)
                    .HasMaxLength(10000);

                analysis.OwnsMany(a => a.Problems, problem =>
                {
                    problem.Property(pr => pr.Type)
                        .HasConversion<string>();

                    problem.Property(pr => pr.Description)
                        .HasMaxLength(500);

                    problem.Property(pr => pr.Severity)
                        .HasMaxLength(50);
                });

                analysis.Property(a => a.Recommendations)
                    .HasConversion(
                        v => string.Join(";", v),
                        v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                    )
                    .HasMaxLength(2000);
            });

            // Complex type for Interactions
            entity.OwnsMany(p => p.Interactions, interaction =>
            {
                interaction.Property(i => i.Id)
                    .ValueGeneratedNever();

                interaction.Property(i => i.UserQuery)
                    .HasMaxLength(1000);

                interaction.Property(i => i.AIResponse)
                    .HasMaxLength(10000);

                interaction.Property(i => i.Type)
                    .HasConversion<string>();
            });

            // Tags as simple string collection
            entity.Property(p => p.Tags)
                .HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .HasMaxLength(500);

            // Indexes
            entity.HasIndex(p => p.UserId);
            entity.HasIndex(p => p.CreatedAt);
            entity.HasIndex(p => p.Status);
        });
    }
}