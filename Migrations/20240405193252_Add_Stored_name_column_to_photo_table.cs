using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rihal_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class Add_Stored_name_column_to_photo_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoredName",
                table: "Photos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredName",
                table: "Photos");
        }
    }
}
