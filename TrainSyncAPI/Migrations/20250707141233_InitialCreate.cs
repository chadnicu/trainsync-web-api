using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exercise",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    weight_unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Kg"),
                    intensity_unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Rir"),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_exercise_title_user_id",
                table: "exercise",
                columns: new[] { "title", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise");
        }
    }
}
