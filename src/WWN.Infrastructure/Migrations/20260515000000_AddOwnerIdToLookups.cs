using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Spells",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Arts",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "FocusDefinitions",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "ClassAbilityDefinitions",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Spells_OwnerId",
                table: "Spells",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Arts_OwnerId",
                table: "Arts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FocusDefinitions_OwnerId",
                table: "FocusDefinitions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAbilityDefinitions_OwnerId",
                table: "ClassAbilityDefinitions",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Spells_OwnerId", table: "Spells");
            migrationBuilder.DropIndex(name: "IX_Arts_OwnerId", table: "Arts");
            migrationBuilder.DropIndex(name: "IX_FocusDefinitions_OwnerId", table: "FocusDefinitions");
            migrationBuilder.DropIndex(name: "IX_ClassAbilityDefinitions_OwnerId", table: "ClassAbilityDefinitions");

            migrationBuilder.DropColumn(name: "OwnerId", table: "Spells");
            migrationBuilder.DropColumn(name: "OwnerId", table: "Arts");
            migrationBuilder.DropColumn(name: "OwnerId", table: "FocusDefinitions");
            migrationBuilder.DropColumn(name: "OwnerId", table: "ClassAbilityDefinitions");
        }
    }
}
