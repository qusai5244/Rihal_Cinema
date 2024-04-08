using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rihal_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class Add_TakenAt_Column_In_Memory_Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TakenOn",
                table: "Memories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakenOn",
                table: "Memories");
        }
    }
}
