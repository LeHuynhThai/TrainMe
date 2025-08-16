using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRandomExerciseData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RandomExercises",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Push-ups" },
                    { 2, "Squats" },
                    { 3, "Jumping Jacks" },
                    { 4, "Plank" },
                    { 5, "Burpees" },
                    { 6, "Lunges" },
                    { 7, "Mountain Climbers" },
                    { 8, "High Knees" },
                    { 9, "Bicycle Crunches" },
                    { 10, "Wall Sit" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RandomExercises",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }
    }
}
