using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopQuanAo.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "admin123");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$kO3YqZ8qV0jXn3yxLvXHXOxvL4aE5p8KfZvz6YqE3rJ5X4wN7aZYS");

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "FullName", "PasswordHash", "Username" },
                values: new object[] { 2, "Quản trị viên", "admin123", "admin" });
        }
    }
}
