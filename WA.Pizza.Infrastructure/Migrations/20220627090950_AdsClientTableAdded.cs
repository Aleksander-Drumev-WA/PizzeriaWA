using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WA.Pizza.Infrastructure.Migrations
{
    public partial class AdsClientTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdsClientId",
                table: "Advertisements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AdsClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ApiKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdsClients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_AdsClientId",
                table: "Advertisements",
                column: "AdsClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AdsClients_ApiKey",
                table: "AdsClients",
                column: "ApiKey",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_AdsClients_AdsClientId",
                table: "Advertisements",
                column: "AdsClientId",
                principalTable: "AdsClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_AdsClients_AdsClientId",
                table: "Advertisements");

            migrationBuilder.DropTable(
                name: "AdsClients");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_AdsClientId",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "AdsClientId",
                table: "Advertisements");
        }
    }
}
