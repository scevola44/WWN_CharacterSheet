using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCombatSkillToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CombatSkill",
                table: "Items",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CombatSkill",
                table: "Items");
        }
    }
}
