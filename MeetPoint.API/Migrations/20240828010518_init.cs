using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetPoint.API.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    category_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    organizer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ubication = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    publication_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_events_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "dbo",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_events_users_organizer_id",
                        column: x => x.organizer_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendances",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    event_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    state = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendances", x => x.id);
                    table.ForeignKey(
                        name: "FK_attendances_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "dbo",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_attendances_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "comments",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    event_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    content = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    publication_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_events_event_id",
                        column: x => x.event_id,
                        principalSchema: "dbo",
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comments_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "dbo",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendances_event_id",
                schema: "dbo",
                table: "attendances",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_attendances_user_id",
                schema: "dbo",
                table: "attendances",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_event_id",
                schema: "dbo",
                table: "comments",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_comments_user_id",
                schema: "dbo",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_category_id",
                schema: "dbo",
                table: "events",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_organizer_id",
                schema: "dbo",
                table: "events",
                column: "organizer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendances",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "comments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "events",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "users",
                schema: "dbo");
        }
    }
}
