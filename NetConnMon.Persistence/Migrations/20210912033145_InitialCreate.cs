using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetConnMon.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SmtpHost = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Port = table.Column<ushort>(type: "INTEGER", nullable: false),
                    UseSSL = table.Column<bool>(type: "INTEGER", nullable: false),
                    SMTPUsername = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SMTPPassword = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    SenderName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SenderEmail = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    RecipientName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    RecipientEmail = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    TextFormat = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Protocol = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeoutMSec = table.Column<uint>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<ushort>(type: "INTEGER", nullable: false),
                    CheckIntervalSec = table.Column<ushort>(type: "INTEGER", nullable: false),
                    MinInterruptionSec = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BackoffMinSec = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BackoffMaxSec = table.Column<ushort>(type: "INTEGER", nullable: false),
                    BackoffStepSec = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ShouldEmailStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConsequtiveErrorsBeforeDisconnected = table.Column<ushort>(type: "INTEGER", nullable: false),
                    SaveInterval = table.Column<uint>(type: "INTEGER", nullable: false),
                    BackoffSec = table.Column<int>(type: "INTEGER", nullable: false),
                    CanConnect = table.Column<bool>(type: "INTEGER", nullable: true),
                    Disabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    RunningSince = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DownSince = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpSince = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastEmailed = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastErrored = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Errors = table.Column<long>(type: "INTEGER", nullable: false),
                    LastErrorMsg = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UpDownEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestDefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Started = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ended = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Heartbeats = table.Column<int>(type: "INTEGER", nullable: false),
                    IsUpEvent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Errors = table.Column<long>(type: "INTEGER", nullable: false),
                    ConsecutiveErrors = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpDownEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UpDownEvent_TestDefinitions_TestDefinitionId",
                        column: x => x.TestDefinitionId,
                        principalTable: "TestDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UpDownEvent_TestDefinitionId",
                table: "UpDownEvent",
                column: "TestDefinitionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSettings");

            migrationBuilder.DropTable(
                name: "UpDownEvent");

            migrationBuilder.DropTable(
                name: "TestDefinitions");
        }
    }
}
