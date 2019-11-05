using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SSASLogBase.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Server",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Server", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Database",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SSASServerID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Database", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Database_Server_SSASServerID",
                        column: x => x.SSASServerID,
                        principalTable: "Server",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Refresh",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    RefreshType = table.Column<int>(nullable: false),
                    RefreshStatus = table.Column<int>(nullable: false),
                    DatabaseID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refresh", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Refresh_Database_DatabaseID",
                        column: x => x.DatabaseID,
                        principalTable: "Database",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    MessageType = table.Column<string>(nullable: true),
                    RefreshID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Message_Refresh_RefreshID",
                        column: x => x.RefreshID,
                        principalTable: "Refresh",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    MessagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Location_Message_MessagId",
                        column: x => x.MessagId,
                        principalTable: "Message",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SourceObject",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Table = table.Column<string>(nullable: true),
                    Column = table.Column<string>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceObject", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SourceObject_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Database_SSASServerID",
                table: "Database",
                column: "SSASServerID");

            migrationBuilder.CreateIndex(
                name: "IX_Location_MessagId",
                table: "Location",
                column: "MessagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_RefreshID",
                table: "Message",
                column: "RefreshID");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh_DatabaseID",
                table: "Refresh",
                column: "DatabaseID");

            migrationBuilder.CreateIndex(
                name: "IX_SourceObject_LocationId",
                table: "SourceObject",
                column: "LocationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceObject");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Refresh");

            migrationBuilder.DropTable(
                name: "Database");

            migrationBuilder.DropTable(
                name: "Server");
        }
    }
}
