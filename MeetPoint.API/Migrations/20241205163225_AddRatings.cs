using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetPoint.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ratings",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rater_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    event_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    organizer_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    score = table.Column<decimal>(type: "decimal(2,1)", precision: 2, scale: 1, nullable: false),
                    rating_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.id);
                    table.ForeignKey(
                        name: "FK_ratings_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "dbo",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ratings_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ratings_users_organizer_id",
                        column: x => x.organizer_id,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ratings_users_rater_id",
                        column: x => x.rater_id,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ratings_users_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ratings_created_by",
                schema: "dbo",
                table: "ratings",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_event_id",
                schema: "dbo",
                table: "ratings",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_organizer_id",
                schema: "dbo",
                table: "ratings",
                column: "organizer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_rater_id",
                schema: "dbo",
                table: "ratings",
                column: "rater_id");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_updated_by",
                schema: "dbo",
                table: "ratings",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ratings",
                schema: "dbo");
        }
    }
}
