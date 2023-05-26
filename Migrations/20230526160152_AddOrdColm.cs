using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleApp.Migrations
{
    public partial class AddOrdColm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ord",
                schema: "public",
                table: "Users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ord",
                schema: "public",
                table: "Users");
        }
    }
}
