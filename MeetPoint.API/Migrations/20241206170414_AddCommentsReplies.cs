using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetPoint.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "dbo",
                table: "comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comments_parent_id",
                schema: "dbo",
                table: "comments",
                column: "parent_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_parent_id",
                schema: "dbo",
                table: "comments",
                column: "parent_id",
                principalSchema: "dbo",
                principalTable: "comments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_parent_id",
                schema: "dbo",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_parent_id",
                schema: "dbo",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "dbo",
                table: "comments");
        }
    }
}
