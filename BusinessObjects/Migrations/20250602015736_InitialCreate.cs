using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ArticleId);
                    table.ForeignKey(
                        name: "FK_Articles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTag",
                columns: table => new
                {
                    ArticleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTag", x => new { x.ArticleId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ArticleTag_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "ArticleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "IsActive", "ModifiedAt", "ParentCategoryId" },
                values: new object[,]
                {
                    { 1, "Tin Tức", new DateTime(2025, 5, 3, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9955), "Tin tức tổng hợp", true, null, null },
                    { 4, "Thể Thao", new DateTime(2025, 5, 8, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9961), "Tin tức thể thao", true, null, null }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "CreatedAt", "Description", "ModifiedAt", "TagName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 3, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9985), "Tin nóng", null, "Hot" },
                    { 2, new DateTime(2025, 5, 3, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9987), "Xu hướng", null, "Trending" },
                    { 3, new DateTime(2025, 5, 5, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9988), "Trí tuệ nhân tạo", null, "AI" },
                    { 4, new DateTime(2025, 5, 5, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9990), "Lập trình", null, "Programming" },
                    { 5, new DateTime(2025, 5, 8, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9991), "Giáo dục", null, "Education" },
                    { 6, new DateTime(2025, 5, 13, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9992), "Thể thao", null, "Sports" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "Password", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 3, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9833), "admin@gmail.com", "Tran Ngoc Diep", true, null, "123", 0 },
                    { 2, new DateTime(2025, 5, 8, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9854), "staff@gmail.com", "Nguyễn Văn Nam", true, null, "123", 1 },
                    { 3, new DateTime(2025, 5, 13, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9856), "lecturer@gmail.com", "Trần Thị Lan", true, null, "123", 2 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "IsActive", "ModifiedAt", "ParentCategoryId" },
                values: new object[,]
                {
                    { 2, "Công Nghệ", new DateTime(2025, 5, 3, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9958), "Tin tức về công nghệ", true, null, 1 },
                    { 3, "Giáo Dục", new DateTime(2025, 5, 3, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9959), "Tin tức giáo dục", true, null, 1 },
                    { 5, "Bóng Đá", new DateTime(2025, 5, 8, 8, 57, 35, 918, DateTimeKind.Local).AddTicks(9962), "Tin tức bóng đá", true, null, 4 }
                });

            migrationBuilder.InsertData(
                table: "Articles",
                columns: new[] { "ArticleId", "CategoryId", "Content", "CreatedAt", "CreatedBy", "Headline", "ModifiedAt", "ModifiedBy", "Source", "Status", "Title" },
                values: new object[,]
                {
                    { "ART001", 2, "Trong năm 2025, AI đã trở thành một phần không thể thiếu trong cuộc sống hàng ngày. Từ việc tự động hóa quy trình làm việc đến hỗ trợ ra quyết định, AI đang mang lại những thay đổi tích cực...", new DateTime(2025, 5, 18, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(15), 2, "Trí tuệ nhân tạo đang thay đổi cách chúng ta làm việc", new DateTime(2025, 5, 23, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(17), 2, "FUNews Tech", "Published", "Xu hướng AI trong năm 2025" },
                    { "ART002", 3, "Hệ thống giáo dục đại học đang trải qua những thay đổi lớn nhằm phù hợp với thời đại số. Các trường đại học đang áp dụng công nghệ mới để nâng cao chất lượng giảng dạy...", new DateTime(2025, 5, 21, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(21), 3, "Những thay đổi mới trong hệ thống giáo dục", new DateTime(2025, 5, 25, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(22), 3, "FUNews Education", "Published", "Cải cách giáo dục đại học" },
                    { "ART003", 5, "World Cup 2026 sẽ là giải đấu bóng đá lớn nhất thế giới với sự tham gia của 48 đội tuyển. Các đội tuyển đang tích cực chuẩn bị cho giải đấu này...", new DateTime(2025, 5, 25, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(24), 2, "Các đội tuyển đang tích cực chuẩn bị", new DateTime(2025, 5, 28, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(24), 1, "FUNews Sports", "Published", "World Cup 2026 - Chuẩn bị cho giải đấu lớn" },
                    { "ART004", 2, ".NET 8 mang lại nhiều tính năng mới và cải tiến hiệu suất đáng kể. Các nhà phát triển có thể tận dụng những tính năng này để xây dựng ứng dụng hiệu quả hơn...", new DateTime(2025, 5, 28, 8, 57, 35, 919, DateTimeKind.Local).AddTicks(28), 3, "Tính năng mới trong .NET 8", null, null, "FUNews Tech", "Draft", "Lập trình với .NET 8" }
                });

            migrationBuilder.InsertData(
                table: "ArticleTag",
                columns: new[] { "ArticleId", "TagId" },
                values: new object[,]
                {
                    { "ART001", 1 },
                    { "ART001", 2 },
                    { "ART001", 3 },
                    { "ART002", 2 },
                    { "ART002", 5 },
                    { "ART003", 1 },
                    { "ART003", 6 },
                    { "ART004", 3 },
                    { "ART004", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTag_TagId",
                table: "ArticleTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTag");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
