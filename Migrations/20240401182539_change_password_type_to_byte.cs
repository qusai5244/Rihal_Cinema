using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rihal_Cinema.Migrations
{
    /// <inheritdoc />
    public partial class change_password_type_to_byte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" ALTER COLUMN ""Password"" TYPE bytea USING ""Password""::bytea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
