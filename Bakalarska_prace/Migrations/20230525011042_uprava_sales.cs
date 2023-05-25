using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakalarska_prace.Migrations
{
    /// <inheritdoc />
    public partial class uprava_sales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Sale_date",
                table: "Sales",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_User_id",
                table: "Sales",
                column: "User_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Cars_Cars_id",
                table: "Sales",
                column: "Cars_id",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_Customers_id",
                table: "Sales",
                column: "Customers_id",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_User_id",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Cars_Cars_id",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_Customers_id",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Cars_id",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Customers_id",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_User_id",
                table: "Sales");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Sale_date",
                table: "Sales",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }
    }
}
