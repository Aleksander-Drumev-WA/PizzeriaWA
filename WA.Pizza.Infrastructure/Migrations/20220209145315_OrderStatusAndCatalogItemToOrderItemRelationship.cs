using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WA.Pizza.Infrastructure.Migrations
{
    public partial class OrderStatusAndCatalogItemToOrderItemRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_BasketItems_BasketItemId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "BasketItemId",
                table: "OrderItems",
                newName: "CatalogItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_BasketItemId",
                table: "OrderItems",
                newName: "IX_OrderItems_CatalogItemId");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StorageQuantity",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_CatalogItems_CatalogItemId",
                table: "OrderItems",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_CatalogItems_CatalogItemId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StorageQuantity",
                table: "CatalogItems");

            migrationBuilder.RenameColumn(
                name: "CatalogItemId",
                table: "OrderItems",
                newName: "BasketItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_CatalogItemId",
                table: "OrderItems",
                newName: "IX_OrderItems_BasketItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_BasketItems_BasketItemId",
                table: "OrderItems",
                column: "BasketItemId",
                principalTable: "BasketItems",
                principalColumn: "Id");
        }
    }
}
