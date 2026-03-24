using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFocusEffectBonusSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Level1Effects",
                table: "FocusDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level2Effects",
                table: "FocusDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConditionalActive",
                table: "Foci",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level1Effects",
                table: "FocusDefinitions");

            migrationBuilder.DropColumn(
                name: "Level2Effects",
                table: "FocusDefinitions");

            migrationBuilder.DropColumn(
                name: "ConditionalActive",
                table: "Foci");
        }
    }
}
