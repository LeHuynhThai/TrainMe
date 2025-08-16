using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRandomExerciseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RandomExercises_Category",
                table: "RandomExercises");

            migrationBuilder.DropIndex(
                name: "IX_RandomExercises_Difficulty",
                table: "RandomExercises");

            migrationBuilder.DropIndex(
                name: "IX_RandomExercises_IsActive_Category",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Equipment",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RandomExercises");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RandomExercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Calories",
                table: "RandomExercises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "RandomExercises",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RandomExercises",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RandomExercises",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "RandomExercises",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "RandomExercises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Equipment",
                table: "RandomExercises",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "RandomExercises",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RandomExercises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "RandomExercises",
                type: "datetime2",
                nullable: true);

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
    }
}
