using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class FuNewsDbContext : DbContext
{
    public FuNewsDbContext()
    {
    }

    public FuNewsDbContext(DbContextOptions<FuNewsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DefaultConnection"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Articles_CategoryId");

            entity.Property(e => e.Headline).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Articles)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(d => d.Tags).WithMany(p => p.Articles)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticleTag",
                    r => r.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    l => l.HasOne<Article>().WithMany().HasForeignKey("ArticleId"),
                    j =>
                    {
                        j.HasKey("ArticleId", "TagId");
                        j.ToTable("ArticleTag");
                        j.HasIndex(new[] { "TagId" }, "IX_ArticleTag_TagId");
                    });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasIndex(e => e.ParentCategoryId, "IX_Categories_ParentCategoryId");

            entity.Property(e => e.CategoryName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory).HasForeignKey(d => d.ParentCategoryId);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.TagName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
