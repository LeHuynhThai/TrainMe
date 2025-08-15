using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class new3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutItems_UserId_Name_DayOfWeek",
                table: "WorkoutItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "WorkoutItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "WorkoutItems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutItems_UserId_Name_DayOfWeek",
                table: "WorkoutItems",
                columns: new[] { "UserId", "Name", "DayOfWeek" },
                unique: true);
        }
    }
}
