using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Clearing.Server.Migrations
{
    /// <inheritdoc />
    public partial class SqliteInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataStores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<long>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    DataStoreName = table.Column<string>(type: "TEXT", nullable: false),
                    DataStoreKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataStores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RobloxGameKeys",
                columns: table => new
                {
                    GameId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WebHookSecret = table.Column<string>(type: "TEXT", nullable: false),
                    OpenCloudApiKey = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobloxGameKeys", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "RobloxUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    GameIds = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RobloxUsers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataStores");

            migrationBuilder.DropTable(
                name: "RobloxGameKeys");

            migrationBuilder.DropTable(
                name: "RobloxUsers");
        }
    }
}
