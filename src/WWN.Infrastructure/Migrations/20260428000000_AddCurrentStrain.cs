using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WWN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentStrain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStrain",
                table: "Characters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStrain",
                table: "Characters");
        }
    }
}
