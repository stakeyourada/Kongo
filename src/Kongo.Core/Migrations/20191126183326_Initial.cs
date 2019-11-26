using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kongo.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FragmentStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    FragmentsReceviedFromRest = table.Column<long>(nullable: false),
                    FragmentsReceviedFromNetwork = table.Column<long>(nullable: false),
                    FragmentsInBlock = table.Column<long>(nullable: false),
                    FragmentsRejected = table.Column<long>(nullable: false),
                    FragmentsPending = table.Column<long>(nullable: false),
                    TotalFragments = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FragmentStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadersLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    LeadersLogsJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadersLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Msg = table.Column<string>(maxLength: 255, nullable: false),
                    Level = table.Column<string>(maxLength: 10, nullable: false),
                    Ts = table.Column<DateTimeOffset>(nullable: false),
                    Node_id = table.Column<string>(maxLength: 64, nullable: true),
                    Peer_addr = table.Column<string>(maxLength: 25, nullable: false),
                    Task = table.Column<string>(maxLength: 30, nullable: false),
                    Reason = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetworkStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    TotalEstablishedConnections = table.Column<int>(nullable: false),
                    LastBlockReceivedAt = table.Column<DateTimeOffset>(nullable: true),
                    LastFragmentReceivedAt = table.Column<DateTimeOffset>(nullable: true),
                    LastGossipReceivedAt = table.Column<DateTimeOffset>(nullable: true),
                    BlocksReceivedInPast30Min = table.Column<int>(nullable: false),
                    FragmentsReceivedInPast30Min = table.Column<int>(nullable: false),
                    GossipReceivedInPast30Min = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeStatisticEntries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    BlockRecvCnt = table.Column<long>(nullable: false),
                    LastBlockDate = table.Column<string>(maxLength: 20, nullable: false),
                    LastBlockFees = table.Column<long>(nullable: false),
                    LastBlockHash = table.Column<string>(maxLength: 64, nullable: true),
                    LastBlockHeight = table.Column<string>(maxLength: 20, nullable: true),
                    lastBlockSum = table.Column<long>(nullable: false),
                    LastBlockTime = table.Column<DateTimeOffset>(nullable: true),
                    LastBlockTx = table.Column<long>(nullable: false),
                    TxRecvCnt = table.Column<long>(nullable: false),
                    Uptime = table.Column<long>(nullable: false),
                    State = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeStatisticEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StakeDistribution",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Epoch = table.Column<long>(nullable: false),
                    Dangling = table.Column<long>(nullable: false),
                    PoolDistributionJson = table.Column<string>(nullable: true),
                    Unassigned = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakeDistribution", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FragmentStatistics_Timestamp",
                table: "FragmentStatistics",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LeadersLogs_Timestamp",
                table: "LeadersLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Ts",
                table: "LogEntries",
                column: "Ts");

            migrationBuilder.CreateIndex(
                name: "IX_NetworkStatistics_Timestamp",
                table: "NetworkStatistics",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_NodeStatisticEntries_Timestamp",
                table: "NodeStatisticEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_StakeDistribution_Timestamp",
                table: "StakeDistribution",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FragmentStatistics");

            migrationBuilder.DropTable(
                name: "LeadersLogs");

            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.DropTable(
                name: "NetworkStatistics");

            migrationBuilder.DropTable(
                name: "NodeStatisticEntries");

            migrationBuilder.DropTable(
                name: "StakeDistribution");
        }
    }
}
