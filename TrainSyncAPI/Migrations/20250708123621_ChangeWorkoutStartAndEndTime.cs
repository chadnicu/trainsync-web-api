using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWorkoutStartAndEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "workout",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "workout",
                newName: "end_time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "workout",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "workout",
                newName: "end_date");
        }
    }
}
