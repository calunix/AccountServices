using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountServices.Migrations
{
    public partial class RemoveEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmailAddress",
                table: "PasswordReset");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmailAddress",
                table: "PasswordReset",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
