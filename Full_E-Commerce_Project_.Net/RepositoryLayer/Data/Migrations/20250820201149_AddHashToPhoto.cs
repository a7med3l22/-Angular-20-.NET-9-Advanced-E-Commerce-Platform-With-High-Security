using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLayer.data.Migrations
{
    /// <inheritdoc />
    public partial class AddHashToPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Photos");
        }
    }
}
