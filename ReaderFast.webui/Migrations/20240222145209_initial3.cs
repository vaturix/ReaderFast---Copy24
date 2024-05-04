using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReaderFast.webui.Migrations
{
    /// <inheritdoc />
    public partial class initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Stories",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Stories",
                newName: "Language");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Difficulty",
                table: "Sentences",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Stories");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Stories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Language",
                table: "Stories",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "Sentences",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
