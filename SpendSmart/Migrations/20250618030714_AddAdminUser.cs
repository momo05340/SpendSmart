using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpendSmart.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            // Seed Admin user
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "Email", "Password", "userrole", "DateCreated", "DateUpdated" },
                values: new object[]
                {
            "Admin",
            "admin@email.com",
            "admin123", 
            "Admin",
            DateTime.Now,
            DateTime.Now
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "userrole",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
