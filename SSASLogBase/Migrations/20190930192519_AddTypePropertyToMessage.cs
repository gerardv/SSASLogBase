using Microsoft.EntityFrameworkCore.Migrations;

namespace SSASLogBase.Migrations
{
    public partial class AddTypePropertyToMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Message");
        }
    }
}
