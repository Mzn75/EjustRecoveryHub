using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EjustRecoveryHub.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationFound = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    DeviceBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceItems_Items_Id",
                        column: x => x.Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IdName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdItems_Items_Id",
                        column: x => x.Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JewelryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    JewelryType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JewelryMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JewelryDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JewelryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JewelryItems_Items_Id",
                        column: x => x.Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotebookItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    NotebookColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotebookDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotebookItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotebookItems_Items_Id",
                        column: x => x.Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WalletColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletBrandOrMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletItems_Items_Id",
                        column: x => x.Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceItems");

            migrationBuilder.DropTable(
                name: "IdItems");

            migrationBuilder.DropTable(
                name: "JewelryItems");

            migrationBuilder.DropTable(
                name: "NotebookItems");

            migrationBuilder.DropTable(
                name: "WalletItems");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
