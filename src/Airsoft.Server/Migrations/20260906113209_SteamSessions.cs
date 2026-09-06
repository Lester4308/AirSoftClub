using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airsoft.Server.Migrations
{
    /// <inheritdoc />
    public partial class SteamSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionRow",
                columns: table => new
                {
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    TicketHash = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Until = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionRow", x => x.TokenHash);
                    table.ForeignKey(
                        name: "FK_SessionRow_Clubs_Owner",
                        column: x => x.Owner,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionRow_Owner",
                table: "SessionRow",
                column: "Owner");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRow_TicketHash",
                table: "SessionRow",
                column: "TicketHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionRow");
        }
    }
}
