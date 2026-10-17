using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Decks_DeckId",
                table: "Cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Decks",
                table: "Decks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.RenameTable(
                name: "Decks",
                newName: "Deck");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "Card");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_DeckId",
                table: "Card",
                newName: "IX_Card_DeckId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deck",
                table: "Deck",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Card",
                table: "Card",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TelegramUserName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TelegramId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_TelegramUserName",
                table: "Students",
                column: "TelegramUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Deck_DeckId",
                table: "Card",
                column: "DeckId",
                principalTable: "Deck",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Deck_DeckId",
                table: "Card");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deck",
                table: "Deck");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Card",
                table: "Card");

            migrationBuilder.RenameTable(
                name: "Deck",
                newName: "Decks");

            migrationBuilder.RenameTable(
                name: "Card",
                newName: "Cards");

            migrationBuilder.RenameIndex(
                name: "IX_Card_DeckId",
                table: "Cards",
                newName: "IX_Cards_DeckId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Decks",
                table: "Decks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Decks_DeckId",
                table: "Cards",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
