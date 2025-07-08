using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToAnnotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "template",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "weight_unit",
                table: "exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Kilograms",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Kg");

            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RepsInReserve",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Rir");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "template",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "weight_unit",
                table: "exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Kg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Kilograms");

            migrationBuilder.AlterColumn<string>(
                name: "intensity_unit",
                table: "exercise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Rir",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RepsInReserve");
        }
    }
}
