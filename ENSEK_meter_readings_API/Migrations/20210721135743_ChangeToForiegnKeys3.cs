using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ENSEK_meter_readings_API.Migrations
{
    public partial class ChangeToForiegnKeys3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeterReading",
                columns: table => new
                {
                    ReadingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountFk = table.Column<int>(type: "int", nullable: false),
                    MeterReadingDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeterReadingValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReading", x => x.ReadingID);
                    table.ForeignKey(
                        name: "FK_MeterReading_Account_AccountFk",
                        column: x => x.AccountFk,
                        principalTable: "Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeterReading_AccountFk",
                table: "MeterReading",
                column: "AccountFk");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeterReading");
        }
    }
}
