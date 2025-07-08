using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutModelAndContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workout",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    programmed_date = table.Column<DateTime>(type: "date", nullable: false),
                    start_date = table.Column<TimeSpan>(type: "time", nullable: true),
                    end_date = table.Column<TimeSpan>(type: "time", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workout");
        }
    }
}
