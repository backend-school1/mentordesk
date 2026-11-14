using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedDeckToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SelectedDeckId",
                table: "Students",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SelectedDeckId",
                table: "Students",
                column: "SelectedDeckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Decks_SelectedDeckId",
                table: "Students",
                column: "SelectedDeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Decks_SelectedDeckId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SelectedDeckId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SelectedDeckId",
                table: "Students");
        }
    }
}
