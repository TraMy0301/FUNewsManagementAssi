using Microsoft.EntityFrameworkCore;
using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Data
{
    public class FuNewsManagementDbContext : DbContext
    {
        public FuNewsManagementDbContext(DbContextOptions<FuNewsManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ Article - Category (n-1)
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình quan hệ Article - Tag (n-n)
            modelBuilder.Entity<Article>()
                .HasMany(a => a.Tags)
                .WithMany(t => t.Articles)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticleTag",
                    j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    j => j.HasOne<Article>().WithMany().HasForeignKey("ArticleId"),
                    j =>
                    {
                        j.HasKey("ArticleId", "TagId");
                        j.ToTable("ArticleTag");
                    });

            // Cấu hình quan hệ Category - ParentCategory (self-referencing)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.InverseParentCategory)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình enum cho AccountRole
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<int>();


            // Seed data cho Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Tran Ngoc Diep",
                    Email = "admin@gmail.com",
                    Password = "123", // Trong thực tế nên hash password
                    Role = AccountRole.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new User
                {
                    UserId = 2,
                    FullName = "Nguyễn Văn Nam",
                    Email = "staff@gmail.com",
                    Password = "123",
                    Role = AccountRole.Staff,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new User
                {
                    UserId = 3,
                    FullName = "Trần Thị Lan",
                    Email = "lecturer@gmail.com",
                    Password = "123",
                    Role = AccountRole.Lecturer,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-20)
                }
            );

            // Seed data cho Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Tin Tức",
                    Description = "Tin tức tổng hợp",
                    ParentCategoryId = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Công Nghệ",
                    Description = "Tin tức về công nghệ",
                    ParentCategoryId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Giáo Dục",
                    Description = "Tin tức giáo dục",
                    ParentCategoryId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Thể Thao",
                    Description = "Tin tức thể thao",
                    ParentCategoryId = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new Category
                {
                    CategoryId = 5,
                    CategoryName = "Bóng Đá",
                    Description = "Tin tức bóng đá",
                    ParentCategoryId = 4,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-25)
                }
            );

            // Seed data cho Tags
            modelBuilder.Entity<Tag>().HasData(
                new Tag
                {
                    TagId = 1,
                    TagName = "Hot",
                    Description = "Tin nóng",
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Tag
                {
                    TagId = 2,
                    TagName = "Trending",
                    Description = "Xu hướng",
                    CreatedAt = DateTime.Now.AddDays(-30)
                },
                new Tag
                {
                    TagId = 3,
                    TagName = "AI",
                    Description = "Trí tuệ nhân tạo",
                    CreatedAt = DateTime.Now.AddDays(-28)
                },
                new Tag
                {
                    TagId = 4,
                    TagName = "Programming",
                    Description = "Lập trình",
                    CreatedAt = DateTime.Now.AddDays(-28)
                },
                new Tag
                {
                    TagId = 5,
                    TagName = "Education",
                    Description = "Giáo dục",
                    CreatedAt = DateTime.Now.AddDays(-25)
                },
                new Tag
                {
                    TagId = 6,
                    TagName = "Sports",
                    Description = "Thể thao",
                    CreatedAt = DateTime.Now.AddDays(-20)
                }
            );

            // Seed data cho Articles
            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    ArticleId = "ART001",
                    Title = "Xu hướng AI trong năm 2025",
                    Headline = "Trí tuệ nhân tạo đang thay đổi cách chúng ta làm việc",
                    Content = "Trong năm 2025, AI đã trở thành một phần không thể thiếu trong cuộc sống hàng ngày. Từ việc tự động hóa quy trình làm việc đến hỗ trợ ra quyết định, AI đang mang lại những thay đổi tích cực...",
                    Source = "FUNews Tech",
                    CategoryId = 2,
                    Status = "Published",
                    CreatedBy = 2,
                    CreatedAt = DateTime.Now.AddDays(-15),
                    ModifiedBy = 2,
                    ModifiedAt = DateTime.Now.AddDays(-10)
                },
                new Article
                {
                    ArticleId = "ART002",
                    Title = "Cải cách giáo dục đại học",
                    Headline = "Những thay đổi mới trong hệ thống giáo dục",
                    Content = "Hệ thống giáo dục đại học đang trải qua những thay đổi lớn nhằm phù hợp với thời đại số. Các trường đại học đang áp dụng công nghệ mới để nâng cao chất lượng giảng dạy...",
                    Source = "FUNews Education",
                    CategoryId = 3,
                    Status = "Published",
                    CreatedBy = 3,
                    CreatedAt = DateTime.Now.AddDays(-12),
                    ModifiedBy = 3,
                    ModifiedAt = DateTime.Now.AddDays(-8)
                },
                new Article
                {
                    ArticleId = "ART003",
                    Title = "World Cup 2026 - Chuẩn bị cho giải đấu lớn",
                    Headline = "Các đội tuyển đang tích cực chuẩn bị",
                    Content = "World Cup 2026 sẽ là giải đấu bóng đá lớn nhất thế giới với sự tham gia của 48 đội tuyển. Các đội tuyển đang tích cực chuẩn bị cho giải đấu này...",
                    Source = "FUNews Sports",
                    CategoryId = 5,
                    Status = "Published",
                    CreatedBy = 2,
                    CreatedAt = DateTime.Now.AddDays(-8),
                    ModifiedBy = 1,
                    ModifiedAt = DateTime.Now.AddDays(-5)
                },
                new Article
                {
                    ArticleId = "ART004",
                    Title = "Lập trình với .NET 8",
                    Headline = "Tính năng mới trong .NET 8",
                    Content = ".NET 8 mang lại nhiều tính năng mới và cải tiến hiệu suất đáng kể. Các nhà phát triển có thể tận dụng những tính năng này để xây dựng ứng dụng hiệu quả hơn...",
                    Source = "FUNews Tech",
                    CategoryId = 2,
                    Status = "Draft",
                    CreatedBy = 3,
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            );

            // Seed data cho quan hệ many-to-many Article-Tag
            // Sử dụng correct property names cho junction table
            modelBuilder.Entity("ArticleTag").HasData(
                new { ArticleId = "ART001", TagId = 1 }, // AI article - Hot tag
                new { ArticleId = "ART001", TagId = 2 }, // AI article - Trending tag
                new { ArticleId = "ART001", TagId = 3 }, // AI article - AI tag
                new { ArticleId = "ART002", TagId = 2 }, // Education article - Trending tag
                new { ArticleId = "ART002", TagId = 5 }, // Education article - Education tag
                new { ArticleId = "ART003", TagId = 1 }, // World Cup article - Hot tag
                new { ArticleId = "ART003", TagId = 6 }, // World Cup article - Sports tag
                new { ArticleId = "ART004", TagId = 3 }, // .NET article - AI tag
                new { ArticleId = "ART004", TagId = 4 }  // .NET article - Programming tag
            );
        }
    }
}