using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class IntermediaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "template_exercise",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order = table.Column<double>(type: "float", nullable: false),
                    instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    template_id = table.Column<long>(type: "bigint", nullable: false),
                    exercise_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_exercise", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_exercise_exercise_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_template_exercise_template_template_id",
                        column: x => x.template_id,
                        principalTable: "template",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_exercise",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order = table.Column<double>(type: "float", nullable: false),
                    instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    workout_id = table.Column<long>(type: "bigint", nullable: false),
                    exercise_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_exercise", x => x.id);
                    table.ForeignKey(
                        name: "FK_workout_exercise_exercise_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "exercise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workout_exercise_workout_workout_id",
                        column: x => x.workout_id,
                        principalTable: "workout",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_exercise_set",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reps = table.Column<double>(type: "float", nullable: true),
                    weight = table.Column<double>(type: "float", nullable: true),
                    intensity = table.Column<double>(type: "float", nullable: true),
                    weight_unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Kilograms"),
                    intensity_unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "RepsInReserve"),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    template_exercise_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_template_exercise_set", x => x.id);
                    table.ForeignKey(
                        name: "FK_template_exercise_set_template_exercise_template_exercise_id",
                        column: x => x.template_exercise_id,
                        principalTable: "template_exercise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workout_exercise_set",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reps = table.Column<double>(type: "float", nullable: true),
                    weight = table.Column<double>(type: "float", nullable: true),
                    intensity = table.Column<double>(type: "float", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    weight_unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Kilograms"),
                    intensity_unit = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "RepsInReserve"),
                    user_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    workout_exercise_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workout_exercise_set", x => x.id);
                    table.ForeignKey(
                        name: "FK_workout_exercise_set_workout_exercise_workout_exercise_id",
                        column: x => x.workout_exercise_id,
                        principalTable: "workout_exercise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_template_exercise_exercise_id",
                table: "template_exercise",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_exercise_template_id",
                table: "template_exercise",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_template_exercise_set_template_exercise_id",
                table: "template_exercise_set",
                column: "template_exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercise_exercise_id",
                table: "workout_exercise",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercise_workout_id",
                table: "workout_exercise",
                column: "workout_id");

            migrationBuilder.CreateIndex(
                name: "IX_workout_exercise_set_workout_exercise_id",
                table: "workout_exercise_set",
                column: "workout_exercise_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "template_exercise_set");

            migrationBuilder.DropTable(
                name: "workout_exercise_set");

            migrationBuilder.DropTable(
                name: "template_exercise");

            migrationBuilder.DropTable(
                name: "workout_exercise");
        }
    }
}
