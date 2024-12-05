using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetPoint.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReportEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reports",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reporter_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    organizer_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    report_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_reports_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_reports_users_organizer_id",
                        column: x => x.organizer_id,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reports_users_reporter_id",
                        column: x => x.reporter_id,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reports_users_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_reports_created_by",
                schema: "dbo",
                table: "reports",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_reports_organizer_id",
                schema: "dbo",
                table: "reports",
                column: "organizer_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_reporter_id",
                schema: "dbo",
                table: "reports",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_updated_by",
                schema: "dbo",
                table: "reports",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reports",
                schema: "dbo");
        }
    }
}
