using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainMe.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldWorkoutTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop tables in correct order (child tables first due to foreign key constraints)
            migrationBuilder.DropTable(name: "ExerciseLogs");
            migrationBuilder.DropTable(name: "WorkoutExercises");
            migrationBuilder.DropTable(name: "WorkoutLogs");
            migrationBuilder.DropTable(name: "Exercises");
            migrationBuilder.DropTable(name: "Workouts");
            migrationBuilder.DropTable(name: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Note: This Down method is intentionally left empty
            // as recreating the old tables would require complex schema recreation
            // If you need to rollback, restore from backup or manually recreate tables
        }
    }
}
