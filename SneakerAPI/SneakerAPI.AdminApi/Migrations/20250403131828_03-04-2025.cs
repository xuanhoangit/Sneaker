using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SneakerAPI.AdminApi.Migrations
{
    /// <inheritdoc />
    public partial class _03042025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProductColorFile__Main",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductColorFile__Main",
                table: "Files");
        }
    }
}
