using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GymBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class GymSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tg_id = table.Column<long>(type: "bigint", nullable: false),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserBodyMetrics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<long>(type: "bigint", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBodyMetrics", x => x.id);
                    table.CheckConstraint("check_body_weight", "weight > 0");
                    table.ForeignKey(
                        name: "FK_bodymetrics_user",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.id);
                    table.ForeignKey(
                        name: "FK_workouts_user",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    workout_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_exercises_workout",
                        column: x => x.workout_id,
                        principalTable: "Workouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    exercise_d = table.Column<long>(type: "bigint", nullable: false),
                    reps = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.id);
                    table.CheckConstraint("check_sets_reps", "reps >= 0");
                    table.CheckConstraint("check_sets_weight", "weight >= 0");
                    table.ForeignKey(
                        name: "FK_sets_exercise",
                        column: x => x.exercise_d,
                        principalTable: "Exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "id_exercises_name",
                table: "Exercises",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "id_exercises_workout_id",
                table: "Exercises",
                column: "workout_id");

            migrationBuilder.CreateIndex(
                name: "id_sets_exercise_id",
                table: "Sets",
                column: "exercise_d");

            migrationBuilder.CreateIndex(
                name: "id_bodymetrics_date",
                table: "UserBodyMetrics",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "id_bodymetrics_userid",
                table: "UserBodyMetrics",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_id",
                table: "Users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_tg_id",
                table: "Users",
                column: "tg_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idworkout_date",
                table: "Workouts",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_user_id",
                table: "Workouts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "UserBodyMetrics");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
