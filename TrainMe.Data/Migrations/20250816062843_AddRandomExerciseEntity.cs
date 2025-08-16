using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRandomExerciseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RandomExercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Difficulty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RandomExercises", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RandomExercises_Category",
                table: "RandomExercises",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_RandomExercises_Difficulty",
                table: "RandomExercises",
                column: "Difficulty");

            migrationBuilder.CreateIndex(
                name: "IX_RandomExercises_IsActive_Category",
                table: "RandomExercises",
                columns: new[] { "IsActive", "Category" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RandomExercises");
        }
    }
}
