using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DVTechODSS.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandAndArchiveToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedDate",
                table: "InventoryItems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "InventoryItems",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "HighStockLevel",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "InventoryItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedDate",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "HighStockLevel",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "InventoryItems");
        }
    }
}
