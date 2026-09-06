using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace Airsoft.Server.Migrations;

[DbContext(typeof(ClubDb))]
[Migration("20260906142000_LedgerAppendOnly")]
public sealed class LedgerAppendOnly : Migration
{
    protected override void Up(MigrationBuilder b) => b.Sql("""
        CREATE FUNCTION reject_ledger_mutation() RETURNS trigger LANGUAGE plpgsql AS $$
        BEGIN RAISE EXCEPTION 'Wallet ledger is append-only'; END $$;
        CREATE TRIGGER ledger_append_only BEFORE UPDATE OR DELETE ON "Ledger"
        FOR EACH ROW EXECUTE FUNCTION reject_ledger_mutation();
        ALTER TABLE "Clubs" ADD CONSTRAINT state_identity CHECK ("State"->>'Id' = "Id");
        """);
    protected override void Down(MigrationBuilder b) => b.Sql("""
        ALTER TABLE "Clubs" DROP CONSTRAINT state_identity;
        DROP TRIGGER ledger_append_only ON "Ledger";
        DROP FUNCTION reject_ledger_mutation();
        """);
}
