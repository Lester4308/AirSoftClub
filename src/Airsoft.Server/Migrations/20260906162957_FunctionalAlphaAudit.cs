using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airsoft.Server.Migrations
{
    /// <inheritdoc />
    public partial class FunctionalAlphaAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE FUNCTION reject_final_match_mutation() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                  IF OLD."Status" <> 'Pending' THEN RAISE EXCEPTION 'Final battle history is immutable'; END IF;
                  IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Battle history cannot be deleted'; END IF;
                  IF NEW."Input" <> OLD."Input" OR NEW."Policy" <> OLD."Policy" OR NEW."Economy" <> OLD."Economy"
                    OR NEW."Attacker" <> OLD."Attacker" OR NEW."Defender" <> OLD."Defender" THEN
                    RAISE EXCEPTION 'Accepted battle input is immutable';
                  END IF;
                  RETURN NEW;
                END $$;
                CREATE TRIGGER match_history_immutable BEFORE UPDATE OR DELETE ON "Matches"
                FOR EACH ROW EXECUTE FUNCTION reject_final_match_mutation();
                """);
            migrationBuilder.AddColumn<long>(
                name: "AttackerVersion",
                table: "Matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CatalogVersion",
                table: "Matches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "DefenderVersion",
                table: "Matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Settlement",
                table: "Matches",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "At",
                table: "Ledger",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER match_history_immutable ON \"Matches\"; DROP FUNCTION reject_final_match_mutation();");
            migrationBuilder.DropColumn(
                name: "AttackerVersion",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "CatalogVersion",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "DefenderVersion",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Settlement",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "At",
                table: "Ledger");
        }
    }
}
