using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bakalarska_prace.Migrations
{
    /// <inheritdoc />
    public partial class uprava_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Employee", "EMPLOYEE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Customer", "CUSTOMER" });
        }
    }
}
