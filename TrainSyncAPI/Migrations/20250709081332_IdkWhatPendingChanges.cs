using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class IdkWhatPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "workout_exercise_set",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepetitionsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepsInReserve");

            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "template_exercise_set",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepetitionsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepsInReserve");

            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepetitionsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepsInReserve");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "workout_exercise_set",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepetitionsInReserve");

            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "template_exercise_set",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepetitionsInReserve");

            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepetitionsInReserve");
        }
    }
}
