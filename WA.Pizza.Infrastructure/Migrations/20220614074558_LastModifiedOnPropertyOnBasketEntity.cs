using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WA.Pizza.Infrastructure.Migrations
{
    public partial class LastModifiedOnPropertyOnBasketEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Baskets",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Baskets");
        }
    }
}
