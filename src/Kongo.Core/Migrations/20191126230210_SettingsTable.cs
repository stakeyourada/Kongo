using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kongo.Core.Migrations
{
    public partial class SettingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Block0Hash = table.Column<string>(nullable: true),
                    Block0Time = table.Column<DateTimeOffset>(nullable: false),
                    ConsensusVersion = table.Column<string>(nullable: true),
                    CurrSlotStartTime = table.Column<DateTimeOffset>(nullable: false),
                    Certificate = table.Column<long>(nullable: false),
                    Coefficient = table.Column<long>(nullable: false),
                    Constant = table.Column<long>(nullable: false),
                    MaxTxsPerBlock = table.Column<int>(nullable: false),
                    SlotDuration = table.Column<int>(nullable: false),
                    SlotsPerEpoch = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_Timestamp",
                table: "Settings",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
