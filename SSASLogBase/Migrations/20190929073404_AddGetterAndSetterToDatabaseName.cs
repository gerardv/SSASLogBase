using Microsoft.EntityFrameworkCore.Migrations;

namespace SSASLogBase.Migrations
{
    public partial class AddGetterAndSetterToDatabaseName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Database",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Database");
        }
    }
}
