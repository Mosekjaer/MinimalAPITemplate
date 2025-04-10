using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPITemplate.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserHandle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Handle",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Handle",
                table: "AspNetUsers");
        }
    }
}
